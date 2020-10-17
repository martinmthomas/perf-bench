using Analyzer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAnalysisService
    {
        Task<Analysis> GetAnalysisAsync(string platformId, string analysisId);

        bool IsAnalysisInProgress();

        Guid Start(AnalysisRequest request);

        Task<IList<Analysis>> GetLatestAnalysesAsync();
    }
}
