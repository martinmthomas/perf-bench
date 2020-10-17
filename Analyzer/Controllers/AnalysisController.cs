using Analyzer.Models;
using Analyzer.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Analyzer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController
    {
        private readonly IAnalysisService _analysisService;


        public AnalysisController(IAnalysisService analysisService)
        {
            _analysisService = analysisService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartAnalysis([FromBody] AnalysisRequest request)
        {
            if (_analysisService.IsAnalysisInProgress())
                return new BadRequestObjectResult("An analysis is already in progress. Please try again later.");

            var analysisId = _analysisService.Start(request);

            return new OkObjectResult(analysisId);
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalysis([FromQuery] string platformId, [FromQuery] string analysisId)
        {
            return new OkObjectResult(await _analysisService.GetAnalysisAsync(platformId, analysisId));
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestAnalyses()
        {
            return new OkObjectResult(await _analysisService.GetLatestAnalysesAsync());
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            return new OkObjectResult($"Hello at {DateTime.Now:dd/MM/yyyy hh:mm:ss.fff}");
        }
    }
}
