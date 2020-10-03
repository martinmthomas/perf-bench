using System;
using System.Net;

namespace Analyzer.Models
{
    public class Result
    {
        public int RequestId { get; set; }

        public int BatchId { get; set; }

        public long TimeTakenMs { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public HttpStatusCode? Status { get; set; }

        public string ReasonPhrase { get; set; }
    }
}
