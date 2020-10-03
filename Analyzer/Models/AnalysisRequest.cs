namespace Analyzer.Models
{
    public class AnalysisRequest
    {
        public string PlatformId { get; set; }

        public int BatchCount { get; set; }

        public int Total { get; set; }

        public int BatchDurationMs { get; set; }
    }
}
