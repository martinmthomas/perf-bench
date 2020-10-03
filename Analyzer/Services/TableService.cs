using Analyzer.Models.Configs;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class TableService<T> : ITableService<T>
        where T : TableEntity
    {
        private readonly string _connectionString;
        private readonly string _tableName;
        private readonly ILogger<TableService<T>> _logger;

        public CloudTable Table => CloudStorageAccount
            .Parse(_connectionString)
            .CreateCloudTableClient()
            .GetTableReference(_tableName);

        public TableService(IOptions<AnalysisStorageOptions> storageOptions, ILogger<TableService<T>> logger)
        {
            _connectionString = storageOptions.Value.ConnectionString;
            _tableName = storageOptions.Value.TableName;
            _logger = logger;
        }

        public async Task<T> InsertEntityAsync(T entity)
        {
            await CreateIfNotExistsAsync();

            var insertOp = TableOperation.Insert(entity);
            var opResponse = await Table.ExecuteAsync(insertOp);

            LogOperationCost(opResponse);

            return opResponse.Result as T;
        }

        public async Task<T> GetEntityAsync(string partitionKey, string rowKey)
        {
            await CreateIfNotExistsAsync();

            var getOp = TableOperation.Retrieve<T>(partitionKey, rowKey);
            var opResponse = await Table.ExecuteAsync(getOp);

            LogOperationCost(opResponse);

            return opResponse.Result as T;
        }

        private async Task<bool> CreateIfNotExistsAsync()
        {
            return await Table.CreateIfNotExistsAsync();
        }

        private void LogOperationCost(TableResult opResponse)
        {
            if (opResponse.RequestCharge.HasValue)
                _logger.LogInformation($"Request Charge of inserting {(opResponse.Result as TableEntity).RowKey} is {opResponse.RequestCharge.Value}");
        }
    }
}
