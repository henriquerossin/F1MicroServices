using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace F1.RaceAPI.Data
{
    public class ConnectionDB
    {
        public readonly string _connectionString;

        public ConnectionDB(IConfiguration c)
        {
            _connectionString = c.GetConnectionString("DefaultCOnnection");
        }
        public SlqConnection GetSlqConnection()
        {
            return new SlqConnection(_connectionString);
        }
    }
}
