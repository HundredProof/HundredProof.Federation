using System.Data;
using Dapper;
using IdentityEndpoint.DataModels;
using IdentityEndpoint.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Identityendpoing.DataModels
{
    public class SqlLoginHandler
    {
        private IConfiguration _config;
        public string Username { get; set; }
        public string Password { get; set; }

        public SqlLoginHandler(string username, string password, IConfiguration config)
        {
            _config = config;
            Username = username;
            Password = password;
        }
        public bool CheckUser() {
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb")))
            {
                var query = @"SELECT count(*) from users WHERE username = @username";
                var exists = conn.QuerySingle<int>(sql: query, param: new CheckUserParamModel(Username),
                                 commandType: CommandType.Text);
                if (exists == 1) return true;
            }

            return false;
        }

        public SqlUserModel LoginWithUser()
        {
            const string query  = @"SELECT TOP (1) [id],[username],[email],[failedattempts],[enabled]
            FROM [appdb].[dbo].[users] WHERE [username] = @username AND [password] = @password";
            var user = new SqlUserModel();
            using (SqlConnection conn = new SqlConnection(_config.GetConnectionString("TargetAuthDb")))
            {
                user = conn.QuerySingle<SqlUserModel>(sql: query, param: this, commandType: CommandType.Text);
            }

            if (!string.IsNullOrEmpty(user.Username) && user.Enabled)
            {
                return user;
            }

            return null;

        }

        private class CheckUserParamModel
        {
            public string Username { get; set; }

            public CheckUserParamModel(string username)
            {
                Username = username;
            }
        }
    }
    
    
}