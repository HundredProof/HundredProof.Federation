using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace HundredProof.Federation.DataModel {
    public static class PersistanceMigrations {
        private static readonly string filename = "PersistanceMigrations.sql";

        public static string getSql() {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(filename));
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream)) {
                var result = reader.ReadToEnd();
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