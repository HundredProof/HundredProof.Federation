using System;
using HundredProof.Federation.DataModel;
using HundredProof.Federation.DataModel.UserDatabase;
using IdentityServer4.EntityFramework.Storage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityBroker {
    public class SeedData {
        public static void EnsureSeedData(string connectionString) {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            services.AddOperationalDbContext(options => {
                options.ConfigureDbContext = db => {
                    db.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName));
                };
            });
            services.AddConfigurationDbContext(options => {
                options.ConfigureDbContext = db => db.UseSqlServer(connectionString,
                    sql => { sql.MigrationsAssembly(typeof(SeedData).Assembly.FullName); });
            });
            using (var serviceProvider = services.BuildServiceProvider()) {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    Console.WriteLine("Applying User database Migration");
                    context.Database.Migrate();
                }
            }

            Console.WriteLine("Applying Persisted Grants and Configuration database migrations");
            PersistanceMigrations.Migrate(connectionString);
        }
    }
}