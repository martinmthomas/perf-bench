using Analyzer.Models;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IApiService
    {
        Task<Result> GetAsync(int requestIndex, int batchId, string requestUri);
    }
}