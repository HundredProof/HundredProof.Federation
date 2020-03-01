using System.Collections.Generic;
using System.Security.Claims;

namespace HundredProof.Federation.Domain.LegacySqlLoginAdapter
{
    public abstract class BaseSqlUser
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
