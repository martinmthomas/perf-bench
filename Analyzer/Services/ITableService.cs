using Microsoft.Azure.Cosmos.Table;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface ITableService<T>
        where T : TableEntity
    {
        Task<T> GetEntityAsync(string partitionKey, string rowKey);

        Task<T> InsertEntityAsync(T entity);
    }
}