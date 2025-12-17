using Microsoft.Data.SqlClient;

namespace F1.TeamAPI.Data
{
    public class ConnectionDB
    {
        public readonly string _connectionString;

        public ConnectionDB(IConfiguration c)
        {
            _connectionString = c.GetConnectionString("DefaultConnection");
        }
        public SqlConnection GetSlqConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}