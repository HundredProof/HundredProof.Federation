namespace IdentityEndpoint.DataModels
{
    public class SqlUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public short FailedAttempts { get; set; }
        public bool Enabled { get; set; }
    }
}