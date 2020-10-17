using Analyzer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAnalysisRepository
    {
        Task<IList<Analysis>> GetAllAsync();

        Task<Analysis> GetAsync(string platformId, string analysisId);

        Task<IList<PlatformSummary>> GetPlatformsSummaryAsync();

        Task SaveAsync(Analysis analysis);
    }
}
