using Analyzer.Models;
using System;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public interface IAnalysisService
    {
        Task<Analysis> GetAnalysisAsync(string platform, string analysisId);

        bool IsAnalysisInProgress();

        Guid Start(AnalysisRequest request);
    }
}
