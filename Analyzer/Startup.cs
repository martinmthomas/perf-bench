using Analyzer.Models.Configs;
using Analyzer.Models.TableEntities;
using Analyzer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;

namespace Analyzer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureOptions(services);

            services.AddHttpClient<IApiService, ApiService>(client =>
            {
                client.Timeout = TimeSpan.FromMilliseconds(1000);
            });

            services.AddSingleton<IAnalysisRepository, AnalysisRepository>();

            services.AddSingleton<IAnalysisService, AnalysisService>();
            services.AddSingleton<ITableService<AnalysisEntity>, TableService<AnalysisEntity>>();

            services
                .AddControllers()
                .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);
        }

        public void ConfigureOptions(IServiceCollection services)
        {
            services
                .AddOptions<AnalysisStorageOptions>()
                .Bind(Configuration.GetSection("AnalysisStorage"));

            services
                .AddOptions<PlatformsOptions>()
                .Bind(Configuration.GetSection("Platforms"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
