using Analyzer.Models;
using Analyzer.Models.Configs;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class AnalysisRepository : IAnalysisRepository
    {
        private readonly CosmosOptions _cosmosOptions;
        private readonly ILogger<AnalysisRepository> _logger;

        private Database AnalyzerDatabase => new Lazy<Database>(() =>
        {
            if (string.IsNullOrWhiteSpace(_cosmosOptions.ConnectionString) || string.IsNullOrWhiteSpace(_cosmosOptions.DatabaseName))
                throw new ArgumentException("Cosmos database configuration is missing");

            return new CosmosClient(_cosmosOptions.ConnectionString, new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                }
            })
                .CreateDatabaseIfNotExistsAsync(_cosmosOptions.DatabaseName)
                .Result
                .Database;
        }).Value;

        private Container AnalysesContainer => new Lazy<Container>(() =>
        {
            if (string.IsNullOrWhiteSpace(_cosmosOptions.AnalysesContainer?.Name) || string.IsNullOrWhiteSpace(_cosmosOptions.AnalysesContainer?.PartitionKeyPath))
                throw new ArgumentException($"Cosmos container {_cosmosOptions.AnalysesContainer?.Name} configuration is missing");

            return AnalyzerDatabase
                .CreateContainerIfNotExistsAsync(_cosmosOptions.AnalysesContainer.Name, _cosmosOptions.AnalysesContainer.PartitionKeyPath)
                .Result
                .Container;
        }).Value;

        private Container PlatformsContainer => new Lazy<Container>(() =>
        {
            if (string.IsNullOrWhiteSpace(_cosmosOptions.PlatformsContainer?.Name) || string.IsNullOrWhiteSpace(_cosmosOptions.PlatformsContainer?.PartitionKeyPath))
                throw new ArgumentException($"Cosmos container {_cosmosOptions.PlatformsContainer?.Name} configuration is missing");

            return AnalyzerDatabase
                .CreateContainerIfNotExistsAsync(_cosmosOptions.PlatformsContainer.Name, _cosmosOptions.PlatformsContainer.PartitionKeyPath)
                .Result
                .Container;
        }).Value;

        public AnalysisRepository(IOptions<CosmosOptions> cosmosOptions, ILogger<AnalysisRepository> logger)
        {
            if (string.IsNullOrWhiteSpace(cosmosOptions?.Value?.ConnectionString))
                throw new ArgumentException("Incorrect value provided for Cosmos connectionstring");

            _cosmosOptions = cosmosOptions.Value;
            _logger = logger;
        }

        public async Task<IList<Analysis>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        public async Task<Analysis> GetAsync(string platformId, string analysisId)
        {
            var response = await AnalysesContainer.ReadItemAsync<Analysis>(analysisId, new PartitionKey(platformId));

            if (response?.Resource == null)
                throw new ArgumentException($"No analysis could be retrieved for Platform: {platformId} and AnalysisId: {analysisId}. Status: {response.StatusCode}");

            return response.Resource;
        }

        public async Task<IList<PlatformSummary>> GetPlatformsSummaryAsync()
        {
            var platforms = new List<PlatformSummary>();

            var iterator = PlatformsContainer.GetItemLinqQueryable<PlatformSummary>()
                .ToFeedIterator();
            do
            {
                var response = await iterator.ReadNextAsync();
                platforms.AddRange(response.Resource);
            } while (iterator.HasMoreResults);

            return platforms;
        }

        public async Task SaveAsync(Analysis analysis)
        {
            var analysisResponse = await AnalysesContainer.CreateItemAsync(analysis, new PartitionKey(analysis.PlatformId));
            if (!IsSuccessful(analysisResponse.StatusCode))
                throw new Exception($"Analysis for Platform: {analysis.PlatformId}, AnalysisId: {analysis.Id} could not be saved");

            var platformResponse = await PlatformsContainer.UpsertItemAsync(
                new PlatformSummary { Id = analysis.PlatformId, AnalysisId = analysis.Id },
                new PartitionKey(analysis.PlatformId)
                );
            if (!IsSuccessful(platformResponse.StatusCode))
                throw new Exception($"Platform-Analysis (PlatformId: {analysis.PlatformId}, AnalysisId: {analysis.Id}) mapping configuration could not be saved");
        }

        private bool IsSuccessful(HttpStatusCode httpStatusCode) => Convert.ToInt32(httpStatusCode) >= 200 && Convert.ToInt32(httpStatusCode) < 300;
    }
}
