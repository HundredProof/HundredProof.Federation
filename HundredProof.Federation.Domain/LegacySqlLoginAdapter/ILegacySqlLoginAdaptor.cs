using System.Collections.Generic;
using System.Security.Claims;

namespace HundredProof.Federation.Domain.LegacySqlLoginAdapter
{
    public interface ILegacySqlLoginAdapter<T> where T : BaseSqlUser
    {
        string Username { get; set; }
        string Password { get; set; }

        void AddCredentials(string username, string password);
        bool CheckUser();

        T LoginWithUser();

        T AddInvalidLoginAttempt();

        IEnumerable<Claim> GetUserClaims();
    }
}
