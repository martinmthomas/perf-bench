using Analyzer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAnalysisRepository
    {
        Task<IList<Analysis>> GetAllAsync();

        Task<Analysis> GetAsync(string platform, string analysisId);

        Task SaveAsync(Analysis analysis);
    }
}
