using System;
using System.Collections.Generic;

namespace Analyzer.Models
{
    public class Analysis
    {
        public Guid Id { get; set; }

        public string PlatformId { get; set; }

        public IList<Result> Results { get; set; } = new List<Result>();

        public Analysis() { }

        public Analysis(string platformId, Guid analysisId, IList<Result> results)
        {
            PlatformId = platformId;
            Id = analysisId;
            Results = results;
        }
    }
}
