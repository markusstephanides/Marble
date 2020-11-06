using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marble.Core;
using Marble.Logging;
using Marble.Messaging.Rabbit.Extensions;
using Marble.Sandbox.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Marble.SandboxAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddTransient<IMathService, DefaultMathServiceClient>();

            MarbleCore.Builder
                .WithRabbitMessaging()
                .WithLogging()
                .ProvideConfiguration(Configuration)
                .ProvideServiceCollection(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("v1/swagger.json", "CalculatorAPI V1");
                c.ConfigObject.DisplayRequestDuration = true;
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            MarbleCore.Builder
                .ProvideServiceProvider(app.ApplicationServices)
                .BuildAndHost();
        }
    }
}