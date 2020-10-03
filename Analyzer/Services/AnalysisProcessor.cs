using Analyzer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class AnalysisProcessor
    {
        private const int PULSE_DURATION_MS = 1000;

        public AnalysisRequest AnalysisRequest { get; set; }

        public string RequestUrl { get; set; }

        public Func<int, int, string, Task> ExecuteRequestFunc;

        private List<Task> _apiRequestTasks { get; set; } = new List<Task>();

        private Guid _processId = default;

        public async Task<IList<Task>> StartAsync()
        {
            if (_processId != default)
                throw new InvalidOperationException($"An analysis is already in progress. Create a new instance of {nameof(AnalysisProcessor)} to do a new analysis");

            _processId = Guid.NewGuid();

            for (int batchId = 0, currentBatchCount = AnalysisRequest.BatchCount; currentBatchCount <= AnalysisRequest.Total; currentBatchCount += AnalysisRequest.BatchCount)
            {
                await ExecuteBatchAsync(++batchId, currentBatchCount);
            }

            return _apiRequestTasks;
        }

        private async Task ExecuteBatchAsync(int batchId, int numberOfRequests)
        {
            Stopwatch swBatch = Stopwatch.StartNew();

            while (swBatch.ElapsedMilliseconds <= AnalysisRequest.BatchDurationMs)
                await ExecutePulseAsync(batchId, numberOfRequests);
        }

        /// <summary>
        /// Batches are made of Pulses. Each Pulse is a short period, say 1 second, during which a specified number of requests are send to the Api Server.
        /// Once the specified number of requests are sent, system will wait for the remaining time of the short period before returning the list of tasks.
        /// </summary>
        /// <param name="requestStartIndex">The starting id of the requests</param>
        /// <param name="batchId">Id of the parent batch</param>
        /// <param name="requestUrl">Api url</param>
        /// <param name="numberOfRequests">Number of requests to be created</param>
        /// <returns>A list of async tasks corresponding to the Api requests made</returns>
        private async Task ExecutePulseAsync(int batchId, int numberOfRequests)
        {
            Stopwatch swPulse = Stopwatch.StartNew();

            // Do not await the request execution to ensure that system can process responses in the background while new requests are created.
            while (--numberOfRequests >= 0)
            {
                var requestIndex = _apiRequestTasks.Count + 1;
                var requestTask = ExecuteRequestFunc(requestIndex, batchId, RequestUrl);
                _apiRequestTasks.Add(requestTask);
            }

            // wait till Pulse is complete
            if (swPulse.ElapsedMilliseconds < PULSE_DURATION_MS)
                await Task.Delay(PULSE_DURATION_MS - (int)swPulse.ElapsedMilliseconds);
        }
    }
}
