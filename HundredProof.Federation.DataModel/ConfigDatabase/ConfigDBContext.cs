using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace HundredProof.Federation.DataModel.ConfigDatabase {
    public class ConfigDbContext : ConfigurationDbContext {
        public ConfigDbContext(DbContextOptions<ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions)
            : base(options, storeOptions) { }
    }
}