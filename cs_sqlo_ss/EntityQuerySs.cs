using cs_sqlo;
using Microsoft.Data.SqlClient;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using System.Xml;

namespace cs_sqlo_ss
{
    public class EntityQuerySs : EntityQuery
    {

        public EntityQuerySs(Db db, string entity_name) : base(db, entity_name)
        {
        }

        public override List<Dictionary<string, object>> fetch_all()
        {
            using SqlConnection connection = new SqlConnection((string)db.config["connection_string"]);
            connection.Open();
            using SqlCommand command = new SqlCommand(Sql(), connection);
            command.ExecuteNonQuery();
            using SqlDataReader reader = command.ExecuteReader();

            return (List<Dictionary<string, object>>)Utils.Serialize(reader);
        }

        protected override string sql_limit()
        {
            if (size.IsNullOrEmpty()) return "";
            page = page.IsNullOrEmpty() ? 1 : page;
            return "OFFSET " + ((page - 1) * size) + @" ROWS
FETCH FIRST " + size + " ROWS ONLY";
        }
    }
}