using System.Data;
using Dapper;
using HundredProof.Federation.Domain.Account;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace IdentityEndpoint.DataModels {
    public class SqlLoginHandler {
        private readonly IConfiguration _config;

        public SqlLoginHandler(string username, string password, IConfiguration config) {
            _config = config;
            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }

        public bool CheckUser() {
            using (var conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb"))) {
                var query = @"SELECT count(*) from users WHERE username = @username";
                var exists = conn.QuerySingle<int>(query, new CheckUserParamModel(Username),
                    commandType: CommandType.Text);
                if (exists == 1) return true;
            }

            return false;
        }

        public SqlUserModel LoginWithUser() {
            const string query = @"SELECT TOP (1) [id],[username],[email],[failedattempts],[enabled]
            FROM [appdb].[dbo].[users] WHERE [username] = @username AND [password] = @password";
            var user = new SqlUserModel();
            using (var conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb"))) {
                user = conn.QuerySingle<SqlUserModel>(query, this, commandType: CommandType.Text);
            }

            if (!string.IsNullOrEmpty(user.Username) && user.Enabled) return user;

            return null;
        }

        private class CheckUserParamModel {
            public CheckUserParamModel(string username) {
                Username = username;
            }

            public string Username { get; }
        }
    }
}