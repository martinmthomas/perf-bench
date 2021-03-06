﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/plant")]
    public class PlantController
    {
        private static readonly string[] Seasons = new[]
        {
            "Spring", "Summer", "Autumn", "Winter"
        };

        private static readonly string[] PlantNames = new[]
        {
            "Spinach", "Beetroot", "Onion", "Carrot",  "Mint", "Leek"
        };

        public PlantController() { }

        [HttpGet]
        public async Task<IEnumerable<Plant>> Get()
        {
            var rng = new Random();
            var plants = PlantNames.Select(plantName => new Plant
            {
                Name = plantName,
                IdealSeason = Seasons[rng.Next(Seasons.Length)]
            });

            // some asynchronous action.
            await Task.Delay(100);

            return plants;
        }
    }

    public class Plant
    {
        public string Name { get; set; }

        public string IdealSeason { get; set; }
    }
}
