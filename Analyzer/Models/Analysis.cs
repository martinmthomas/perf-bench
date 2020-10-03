using System;
using System.Collections.Generic;

namespace Analyzer.Models
{
    public class Analysis
    {
        public Guid AnalysisId { get; set; }

        public string Platform { get; set; }

        public IList<Result> Results { get; set; } = new List<Result>();

        public Analysis() { }

        public Analysis(string platform, Guid analysisId, IList<Result> results)
        {
            Platform = platform;
            AnalysisId = analysisId;
            Results = results;
        }
    }
}
