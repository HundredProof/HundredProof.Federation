using System;
using System.Collections.Generic;
using System.Text;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace HundredProof.Federation.DataModel.ConfigDatabase {
    public static class IdentityServerConfigDbInitializer {
        public static void InitializeDatabase(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope()) {
                PerformMigrations(serviceScope);
            }
        }

        private static void PerformMigrations(IServiceScope serviceScope) {
            serviceScope.ServiceProvider
                .GetRequiredService<ConfigurationDbContext>()
                .Database
                .Migrate();
        }
    }
}
