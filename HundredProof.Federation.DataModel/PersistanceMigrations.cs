using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Internal;

namespace HundredProof.Federation.DataModel
{
    public static class PersistanceMigrations {
        private static string filename = "PersistanceMigrations.sql";
        public static string getSql() {
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return result;
            }
        }

        public static void Migrate(string connectionString) {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            var cmd = conn.CreateCommand();
            cmd.CommandText = getSql();
            cmd.ExecuteNonQuery();
            conn.Close();
            conn.Dispose();
        }
    }
}