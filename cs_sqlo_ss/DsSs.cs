using cs_sqlo;
using Microsoft.Data.SqlClient;

namespace cs_sqlo_ss
{
    public class DbSs: Db
    {

        SqlConnection _conn;

        /*
         * config["connection_string"] = "server=127.0.0.1;uid=root;pwd=12345;database=test"
         */
        public DbSs(Dictionary<string, object> config) : base(config)
        {
            using SqlConnection _conn = new SqlConnection((string)config["connection_string"]);
            _conn.Open();
        }

        public SqlConnection conn() => _conn;
    }
}