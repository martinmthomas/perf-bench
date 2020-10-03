using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Analyzer.Models.TableEntities
{
    public class AnalysisEntity : TableEntity
    {
        public AnalysisEntity() { }

        public AnalysisEntity(string platform, Guid analysisId, IList<Result> results) : base(platform, analysisId.ToString())
        {
            Platform = platform;
            AnalysisId = analysisId;
            Results = JsonSerializer.Serialize(results, SerializerOptions);
        }

        public AnalysisEntity(Analysis analysis) : this(analysis.Platform, analysis.AnalysisId, analysis.Results) { }

        public Guid AnalysisId { get; set; }

        public string Platform { get; set; }

        public string Results { get; set; }

        public Analysis Map()
        {
            return new Analysis(Platform, AnalysisId, JsonSerializer.Deserialize<IList<Result>>(Results, SerializerOptions));
        }

        private JsonSerializerOptions SerializerOptions => new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
    }
}
