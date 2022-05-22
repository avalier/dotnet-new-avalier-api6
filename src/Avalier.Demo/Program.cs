using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace Avalier.Demo;

public class Program
{
    public static void Main(string[] args)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .WithConfiguration()
            .CreateBootstrapLogger();

        Log.Information("Starting host");

        try
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((ctx, lc) => lc
                .WithConfiguration()
                .ReadFrom.Configuration(ctx.Configuration));

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
        catch (Exception x)
        {
            Log.Fatal(x, "Host terminated (due to exception)");
        }
        finally
        {
            Log.Information("Stopped host");
            Log.CloseAndFlush();
        }
    }
}
