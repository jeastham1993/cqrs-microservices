using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Manipulator.Domain.Models;
using Jeasthamdev.Cqrs.Manipulator.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jeasthamdev.Cqrs.Manipulator
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
            services.AddCustomDbContext(Configuration)
                .AddMassTransitConfig()
                .AddTransient<IOrderRepository, OrderRepository>();

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }

    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<CqrsDataContext>
                            (options => options.UseSqlServer(configuration.GetConnectionString("SqlConnection")));

            return services;
        }

        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(provider =>
            {
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h => { });
                });

                services.AddSingleton<IPublishEndpoint>(bus);
                services.AddSingleton<ISendEndpointProvider>(bus);
                services.AddSingleton<IBus>(bus);

                bus.Start();
            });

            return services;
        }
    }
}
