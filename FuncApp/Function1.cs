using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FuncApp
{
    public static class Function1
    {
        private static readonly string[] Seasons = new[]
        {
            "Spring", "Summer", "Autumn", "Winter"
        };

        private static readonly string[] PlantNames = new[]
        {
            "Spinach", "Beetroot", "Onion", "Carrot",  "Mint", "Leek"
        };

        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "plant")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var rng = new Random();
            var plants = PlantNames.Select(plantName => new Plant
            {
                Name = plantName,
                IdealSeason = Seasons[rng.Next(Seasons.Length)]
            });

            // some asynchronous action.
            await Task.Delay(100);

            return new OkObjectResult(plants);
        }
    }


    public class Plant
    {
        public string Name { get; set; }

        public string IdealSeason { get; set; }
    }
}
