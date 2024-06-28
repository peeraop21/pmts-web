using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace PMTs.DataAccess.Utils
{
    class ConnectionFactory
    {

        public static IDbConnection PresaleConnect(IConfiguration config)
        {
            SqlConnection conn = new SqlConnection(config.GetConnectionString("PresaleConnect"));
            return conn;
        }

    }
}
