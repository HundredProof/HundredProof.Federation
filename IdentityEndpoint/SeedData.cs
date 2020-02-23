using System;
using System.Linq;
using System.Security.Claims;
using HundredProof.Federation.DataModel;
using HundredProof.Federation.DataModel.UserDatabase;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace IdentityEndpoint {
    public class SeedData {
        public static void EnsureSeedData(string connectionString) {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            using (var serviceProvider = services.BuildServiceProvider()) {
                using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope()) {
                    var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                    context.Database.Migrate();

                }
            }
            PersistanceMigrations.Migrate(connectionString);
        }
    }
}