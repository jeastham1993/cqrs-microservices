using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jeasthamdev.Cqrs.Read.Domain.Common;
using Jeasthamdev.Cqrs.Read.Domain.Events;
using Jeasthamdev.Cqrs.Read.Domain.Settings;
using Jeasthamdev.Cqrs.Read.Infrastructure;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jeasthamdev.Cqrs.Read
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
            InstanceSettings.InstanceIdentifier = Guid.NewGuid().ToString();

            DbSettings.ConnectionString = Configuration.GetConnectionString("SqlConnection");

            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = "127.0.0.1";
                option.InstanceName = "master";
            });

            services.AddSingleton<OrderRepository>()
                .AddMassTransitConfig();

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var orderRepository = app.ApplicationServices.GetRequiredService<OrderRepository>();
            orderRepository.GetAndStoreInCacheIfEmpty().Wait();

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
        public static IServiceCollection AddMassTransitConfig(this IServiceCollection services)
        {
            services.AddMassTransit(provider =>
            {
                provider.AddConsumer<NewOrderConsumer>();
                
                var bus = Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    var host = cfg.Host(new Uri("rabbitmq://localhost/"), h => { });

                    cfg.ReceiveEndpoint(host, InstanceSettings.InstanceIdentifier, e =>
                    {
                        e.Consumer<NewOrderConsumer>(services.BuildServiceProvider());
                    });
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
