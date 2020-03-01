using Dapper;
using HundredProof.Federation.Domain;
using HundredProof.Federation.Domain.Account;
using IdentityModel;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Security.Claims;
using HundredProof.Federation.Domain.LegacySqlLoginAdapter;

namespace IdentityEndpoint.DataModels
{
    public class SqlLoginHandler: ILegacySqlLoginAdapter<SqlUserModel> {
        private readonly IConfiguration _config;

        public SqlLoginHandler(IConfiguration config) {
            _config = config;
            Claims = new List<Claim>();
        }


        public string Username { get; set; }
        public string Password { get; set; }
        private List<Claim> Claims { get; set; }

        public void AddCredentials(string username, string password) {
            Username = username;
            Password = password;
        }
        public bool CheckUser() {
            using (var conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb"))) {
                var query = @"SELECT count(*) from users WHERE username = @username";
                var exists = conn.QuerySingle<int>(query, new CheckUserParamModel(Username),
                    commandType: CommandType.Text);
                if (exists == 1) return true;
            }

            return false;
        }

        public BaseSqlUser AddInvalidLoginAttempt() {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Claim> GetUserClaims() {
            return Claims;
        }

        public SqlUserModel LoginWithUser() {
            const string query = @"SELECT TOP (1) [id],[username],[email],[failedattempts],[enabled]
            FROM [appdb].[dbo].[users] WHERE [username] = @username AND [password] = @password";
            var user = new SqlUserModel();
            using (var conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb"))) {
                user = conn.QuerySingle<SqlUserModel>(query, this, commandType: CommandType.Text);
                user.Claims = _getClaimsFromUser(user);
            }

            if (!string.IsNullOrEmpty(user.Username) && user.Enabled) return user;

            return null;
        }

        private List<Claim> _getClaimsFromUser(SqlUserModel user) {
            return new List<Claim> {
                new Claim(JwtClaimTypes.Subject, user.Id)
            };
        }

        SqlUserModel ILegacySqlLoginAdapter<SqlUserModel>.AddInvalidLoginAttempt()
        {
            throw new System.NotImplementedException();
        }

        private class CheckUserParamModel {
            public CheckUserParamModel(string username) {
                Username = username;
            }

            public string Username { get; }
        }
    }
}