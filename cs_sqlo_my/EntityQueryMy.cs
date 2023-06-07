using System.Data.Common;

namespace cs_sqlo
{
    public class EntityQueryMy : EntityQuery
    {

        public EntityQueryMy(Db db, string entity_name) : base(db, entity_name)
        {
        }

        public override DbDataReader execute()
        {
            throw new NotImplementedException();
        }

        protected override string sql_limit()
        {
            if (size.IsNullOrEmpty()) return "";
            page = page.IsNullOrEmpty() ? 1 : page;
            return "LIMIT " + size + " OFFSET " + ((page - 1) * size) + @"
";
        }
    }

}