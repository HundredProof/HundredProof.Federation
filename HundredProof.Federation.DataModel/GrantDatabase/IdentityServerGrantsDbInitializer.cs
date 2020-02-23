using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HundredProof.Federation.DataModel.GrantDatabase {
    public static class IdentityServerGrantsDbInitializer {
        public static void InitializeDatabase(IApplicationBuilder app) {
            using (var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope()) {
                PerformMigrations(serviceScope);
            }
        }

        private static void PerformMigrations(IServiceScope serviceScope) {
            serviceScope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>()
                .Database
                .Migrate();
        }
    }
}