using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Hangfire.MongoSample.Controllers;
using Hangfire.MongoSample.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Hangfire.MongoSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hangfire.MongoSample", Version = "v1" });
            });
            services.Configure<MongoSettings>(x =>
            {
                x.ConnectionString = "mongodb://localhost:27017";
                x.DatabaseName = "hangfire-sample";
            });

            services.AddSingleton<MongoContext>();
            services.AddTransient<RemainderService>();

            services.AddHangfire((sp, configuration) => {
                var mongo = sp.GetRequiredService<MongoContext>();

                configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseFilter(new AutomaticRetryAttribute { Attempts = 0 }) // no retries
                    .UseMongoStorage(mongo.Client, mongo.DatabaseName, new MongoStorageOptions
                    {
                        MigrationOptions = new MongoMigrationOptions
                        {
                            MigrationStrategy = new MigrateMongoMigrationStrategy(),
                            BackupStrategy = new CollectionMongoBackupStrategy()
                        },
                        Prefix = "hangfire.mongo",
                        CheckConnection = true
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                
                // additional configuration
            });
            app.UseHangfireDashboard(); // only local requests to the dashboard are allowed

            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hangfire.MongoSample v1"));

            app.UseRouting();

            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }
}
