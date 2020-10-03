using Analyzer.Models;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace Analyzer.Services
{
    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Result> GetAsync(int requestId, int batchId, string requestUri)
        {
            var swResponse = Stopwatch.StartNew();
            var startTime = DateTimeOffset.UtcNow;

            try
            {
                var response = await _httpClient.GetAsync(requestUri);

                return new Result
                {
                    RequestId = requestId,
                    BatchId = batchId,
                    TimeTakenMs = swResponse.ElapsedMilliseconds,
                    StartTime = startTime,
                    EndTime = DateTimeOffset.UtcNow,
                    Status = response.StatusCode,
                    ReasonPhrase = response.ReasonPhrase
                };
            }
            catch (Exception ex)
            {
                return new Result
                {
                    RequestId = requestId,
                    BatchId = batchId,
                    TimeTakenMs = swResponse.ElapsedMilliseconds,
                    StartTime = startTime,
                    EndTime = DateTimeOffset.UtcNow,
                    Status = null,
                    ReasonPhrase = ex.Message
                };
            }
        }
    }
}
