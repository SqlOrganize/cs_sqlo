using cs_sqlo;
    using Microsoft.Data.SqlClient;

namespace cs_sqlo_ss
{
    public class EntityQuerySs : EntityQuery
    {

        public EntityQuerySs(Db db, string entity_name) : base(db, entity_name)
        {
        }

        public override (string sql, List<object> param, string con) build()
        {
            throw new NotImplementedException();
        }
    }
}