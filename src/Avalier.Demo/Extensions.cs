using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Serilog;
using Serilog.Configuration;
using Serilog.Enrichers.Span;
using Serilog.Formatting.Compact;

namespace Avalier.Demo;

public static class Extensions
{
    [ExcludeFromCodeCoverage]
    public static IServiceCollection AddTransientByConvention(this IServiceCollection services, params Assembly[] assemblies)
    {
        // Default to entry assembly if no assemblies were specified //
        var items = assemblies.ToList();
        if (items.Count == 0)
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (null != entryAssembly) items.Add(entryAssembly);
        }

        // Iterate through assemblies and register classes with matching interfaces //
        foreach (var assembly in assemblies)
        {
            var registrations = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract)
                .Where(t => null != t.GetInterface($"I{t.Name}"))
                .Select(t => new { InterfaceType = t.GetInterface($"I{t.Name}"), ClassType = t })
                .ToList();
                
            foreach (var registration in registrations)
            {
                if (null != registration.InterfaceType)
                {
                    services.AddTransient(registration.InterfaceType, registration.ClassType);
                }
            }
        }
        return services;
    }
    public static LoggerConfiguration WithHostname(this LoggerEnrichmentConfiguration loggerEnrichmentConfiguration)
    {
        return loggerEnrichmentConfiguration.WithProperty(
            "Hostname", 
            System.Environment.GetEnvironmentVariable("HOSTNAME") ?? System.Environment.MachineName ?? ""
        );
    }

    public static LoggerConfiguration WithConfiguration(this LoggerConfiguration LoggerConfiguration)
    {
        return LoggerConfiguration
            .Enrich.FromLogContext()
            .Enrich.WithHostname()
            .Enrich.WithSpan()
            .WriteTo.Console(formatter: new CompactJsonFormatter());
    }
}
