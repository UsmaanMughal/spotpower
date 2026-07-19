using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SpotPower.Infrastructure.Jobs;
using SpotPower.Infrastructure.Omie;
using SpotPower.Infrastructure.Persistence;

namespace SpotPower.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSpotPowerInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OmieOptions>(configuration.GetSection(OmieOptions.SectionName));
        var omieOptions = configuration.GetSection(OmieOptions.SectionName).Get<OmieOptions>() ?? new OmieOptions();

        services.AddDbContext<SpotPowerDbContext>(o =>
            o.UseSqlite(configuration.GetConnectionString("SpotPower")
                ?? throw new InvalidOperationException("Missing 'SpotPower' connection string.")));

        services.AddHttpClient<OmieClient>();

        services.AddQuartz(q =>
        {
            var jobKey = new JobKey(nameof(AuctionImportJob));
            q.AddJob<AuctionImportJob>(opts => opts.WithIdentity(jobKey));

            // Recurring poll on the configured cadence.
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(AuctionImportJob)}-recurring-trigger")
                .WithCronSchedule(omieOptions.ImportCronSchedule));

            // A cron trigger's first fire is the next cron boundary, not "now" - fire once
            // immediately too, so every startup/restart does an immediate catch-up run.
            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity($"{nameof(AuctionImportJob)}-startup-trigger")
                .StartNow());
        });
        services.AddQuartzHostedService(o => o.WaitForJobsToComplete = true);

        return services;
    }
}
