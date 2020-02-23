using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace HundredProof.Federation.DataModel.GrantDatabase {
    public class GrantDbContext : PersistedGrantDbContext {
        public GrantDbContext(DbContextOptions<PersistedGrantDbContext> options, OperationalStoreOptions storeOptions) :
            base(options, storeOptions) { }
    }
}