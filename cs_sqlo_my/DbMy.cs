using cs_sqlo;
using MySql.Data.MySqlClient;

namespace cs_sqlo_my
{

    public class DbMy : Db
    {

        MySqlConnection _conn;

        /*
         * config["connection_string"] = "server=127.0.0.1;uid=root;pwd=12345;database=test"
         */
        public DbMy(Dictionary<string, object> config): base(config)
        {
            _config = config;
            _conn = new MySqlConnection();
            _conn.ConnectionString = (string)config["connection_string"];
            _conn.Open();
        }

        public MySqlConnection conn() => _conn;

    }


}