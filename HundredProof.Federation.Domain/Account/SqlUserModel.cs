using HundredProof.Federation.Domain.LegacySqlLoginAdapter;

namespace HundredProof.Federation.Domain.Account {
    public class SqlUserModel : BaseSqlUser {
        public short FailedAttempts { get; set; }
        public bool Enabled { get; set; }
    }
}