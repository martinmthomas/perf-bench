using Analyzer.Models;
using Analyzer.Models.Configs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class AnalysisService : IAnalysisService
    {
        //private const int PULSE_DURATION_MS = 1000;
        private static Guid _analysisId;

        private readonly ILogger<AnalysisService> _logger;
        private readonly IApiService _apiService;
        private readonly IOptions<PlatformsOptions> _platforms;
        private readonly IAnalysisRepository _analysisRepository;

        private static ConcurrentBag<Result> _results { get; set; }

        public AnalysisService(
            ILogger<AnalysisService> logger,
            IApiService apiService,
            IOptions<PlatformsOptions> platforms,
            IAnalysisRepository analysisRepository)
        {
            _logger = logger;
            _apiService = apiService;
            _platforms = platforms;
            _analysisRepository = analysisRepository;
        }


        public Guid Start(AnalysisRequest request)
        {
            InitAnalysis(request.PlatformId);

            Task.Run(async () =>
            {
                try
                {
                    var platform = GetPlatform(request.PlatformId);

                    var processor = new AnalysisProcessor
                    {
                        AnalysisRequest = request,
                        RequestUrl = platform.Url,
                        ExecuteRequestFunc = ExecuteRequestAsync
                    };

                    var tasks = await processor.StartAsync();

                    await Task
                        .WhenAll(tasks)
                        .ContinueWith(async t => await SaveAnalysisAsync(request.PlatformId));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Starting analysis failed for Platform: {request.PlatformId}, AnalysisId: {_analysisId}");
                    _analysisId = default;
                }
            });

            return _analysisId;
        }

        private void InitAnalysis(string platformId)
        {
            if (IsAnalysisInProgress())
                throw new InvalidOperationException("An analysis is already in progress. Please try again later.");

            _analysisId = Guid.NewGuid();
            _results = new ConcurrentBag<Result>();

            _logger.LogInformation($"Starting analysis for Platform: {platformId}, AnalysisId: {_analysisId}");
        }

        private async Task SaveAnalysisAsync(string platformId)
        {
            try
            {
                var analysis = new Analysis(platformId, _analysisId, _results.ToList());
                await _analysisRepository.SaveAsync(analysis);

                _logger.LogInformation($"Analysis completed for Platform: {platformId}, AnalysisId: {_analysisId}");

                _analysisId = default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Saving analysis results failed");
            }
        }

        public async Task<Analysis> GetAnalysisAsync(string platformId, string analysisId)
        {
            if (IsAnalysisInProgress())
                return new Analysis(platformId, Guid.Parse(analysisId), _results.ToList());

            return await _analysisRepository.GetAsync(platformId, analysisId);
        }

        public bool IsAnalysisInProgress()
        {
            return _analysisId != default(Guid);
        }

        private async Task ExecuteRequestAsync(int requestId, int batchId, string requestUri)
        {
            _results.Add(await _apiService.GetAsync(requestId, batchId, requestUri));
        }

        private Platform GetPlatform(string platformId)
        {
            return _platforms.Value.First(p => p.Id.Equals(platformId, StringComparison.OrdinalIgnoreCase));
        }
    }
}
