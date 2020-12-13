using Marble.Core;
using Marble.Messaging.Rabbit.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Marble.SandboxAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            // services.AddTransient<IMathService, DefaultMathServiceClient>();

            MarbleCore.Builder
                .WithRabbitMessaging()
                .ProvideConfiguration(this.Configuration)
                .ProvideServiceCollection(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("v1/swagger.json", "CalculatorAPI V1");
                c.ConfigObject.DisplayRequestDuration = true;
            });

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            MarbleCore.Builder
                .ProvideServiceProvider(app.ApplicationServices)
                .BuildAndHost();
        }
    }
}