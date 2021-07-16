using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;
using MetricsCommon.Jobs;
using AutoMapper;
using FluentMigrator.Runner;
using Quartz;
using Quartz.Spi;
using Quartz.Impl;
using MetricsManager.DAL;
using MetricsManager.DAL.Repositories;
using MetricsManager.DAL.Interfaces;
using MetricsManager.DAL.Models;
using MetricsManager.Jobs;
using MetricsManager.Client;
using Polly;
using Dapper;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MetricsManager
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
            services.AddControllers();
            services.AddSingleton<IAgentsRepository, AgentsRepository>();
            services.AddSingleton<ICpuMetricsRepositoryApi, CpuMetricsRepositoryApi>();
            services.AddSingleton<IGcHeapSizeMetricsRepositoryApi, GcHeapSizeMetricsRepositoryApi>();
            services.AddSingleton<IHddMetricsRepositoryApi, HddMetricsRepositoryApi>();
            services.AddSingleton<INetworkMetricsRepositoryApi, NetworkMetricsRepositoryApi>();
            services.AddSingleton<IRamMetricsRepositoryApi, RamMetricsRepositoryApi>();
            
            services.AddHttpClient<IMetricsAgentClient, MetricsAgentClient>()
                .AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(1000)));

            var mapperConfiguration = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile()));
            var mapper = mapperConfiguration.CreateMapper();
            services.AddSingleton(mapper);

            services.AddFluentMigratorCore()
                .ConfigureRunner(rb => rb
                    .WithGlobalConnectionString(SqlConntectionParameters.сonnection)
                    .AddSQLite()
                    .ScanIn(typeof(Startup).Assembly).For.Migrations()
                ).AddLogging(lb => lb
                    .AddFluentMigratorConsole());

            services.AddSingleton<IJobFactory, SingletonJobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddSingleton<CpuMetricJobApi>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(CpuMetricJobApi),
                cronExpression: "0/5 * * * * ?"));

            services.AddSingleton<GcHeapSizeMetricJobApi>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(GcHeapSizeMetricJobApi),
                cronExpression: "0/5 * * * * ?"));

            services.AddSingleton<HddMetricJobApi>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(HddMetricJobApi),
                cronExpression: "0/5 * * * * ?"));

            services.AddSingleton<NetworkMetricJobApi>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(NetworkMetricJobApi),
                cronExpression: "0/5 * * * * ?"));

            services.AddSingleton<RamMetricJobApi>();
            services.AddSingleton(new JobSchedule(
                jobType: typeof(RamMetricJobApi),
                cronExpression: "0/5 * * * * ?"));

            services.AddHostedService<QuartzHostedService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API сервиса менеджеора сбора метрик",
                    Description = "Здесь можно протестировать api предлагаемого менеджера метрик",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Lipunov M",
                        Email = string.Empty,
                        Url = new Uri("https://www.facebook.com/mmlipunov/"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Open Source license",
                        Url = new Uri("https://example.com/license"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMigrationRunner migrationRunner)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            migrationRunner.MigrateUp();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API сервиса менеджера сбора метрик");
                c.RoutePrefix = string.Empty;
            });
        }
    }
}
