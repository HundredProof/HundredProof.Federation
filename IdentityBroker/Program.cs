using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;
using System.Linq;

namespace IdentityBroker {
    public class Program {
        public static int Main(string[] args) {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                // uncomment to write to Azure diagnostics stream
                //.WriteTo.File(
                //    @"D:\home\LogFiles\Application\identityserver.txt",
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            try {
                var seed = args.Contains("/seed");
                if (seed) {
                    args = args.Except(new[] {"/seed"}).ToArray();
                }

                var host = CreateHostBuilder(args).Build();

                if (seed) {
                    Log.Information("Seeding database...");
                    var config = host.Services.GetRequiredService<IConfiguration>();
                    var connectionString = config.GetConnectionString("DefaultConnection");
                    SeedData.EnsureSeedData(connectionString);
                    Log.Information("Done seeding database.");
                    return 0;
                }

                Log.Information("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex) {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => {
                    webBuilder.ConfigureKestrel(serverOptions => {
                        serverOptions.ConfigureEndpointDefaults(listenOptions => {
                            var config = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("https.json", optional: true)
                                .AddCommandLine(args)
                                .Build().GetSection("HttpsSettings");
                            listenOptions.UseHttps(System.Security.Cryptography.X509Certificates.StoreName.My,
                                config.GetValue<string>("CertificateSubject"));
                        });
                    });
                    webBuilder.UseUrls("https://broker.example-1.getthinktank.com:443/");
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog();
                });
    }
}
