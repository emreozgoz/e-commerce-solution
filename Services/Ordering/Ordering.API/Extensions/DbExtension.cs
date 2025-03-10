using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;
using System.Runtime.CompilerServices;

namespace Ordering.API.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<Tcontext>(this IHost host, Action<Tcontext, IServiceProvider> seeder) where Tcontext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Tcontext>>();
                var contex = services.GetService<Tcontext>();
                try
                {
                    logger.LogInformation($"Started Db Migrationb: {typeof(Tcontext).Name}");
                    var retry = Policy.Handle<SqlException>()
                        .WaitAndRetry(
                        retryCount: 5,
                        sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                        onRetry: (exception, span, count) =>
                        {
                            logger.LogError($"Retrying because of {exception} {span}");
                        });
                    retry.Execute(() => CallSeeder(seeder, contex, services));
                    logger.LogInformation($"Migration Completed: {typeof(Tcontext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"An Error occured while migrating db: {typeof(Tcontext).Name}");
                }
            }
            return host;
        }

        private static void CallSeeder<Tcontext>(Action<Tcontext, IServiceProvider> seeder, Tcontext? contex, IServiceProvider services) where Tcontext : DbContext
        {
            contex.Database.Migrate();
            seeder(contex, services);
        }
    }
}
