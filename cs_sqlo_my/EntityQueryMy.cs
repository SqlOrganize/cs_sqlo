using cs_sqlo;

namespace cs_sqlo_my
{
    public class EntityQueryMy : EntityQuery
    {

        public EntityQueryMy(Db db, string entity_name) : base(db, entity_name)
        {
        }

        public override (string sql, List<object> param) build()
        {
            throw new NotImplementedException();
        }

        /*
        Metodo inicial para definir condicion
        */
        protected (string sql, List<object>? param) _sql_cond(List<object> condition)
        {
            if (condition.IsNullOrEmpty())
                return ("", null);

            var r = _sql_cond_recursive(condition);
            return (r.sql, r.param);

        }

        /*
        Metodo recursivo para definir condicion

        Si en la posicion 0 es un string significa que es un campo a buscar, 
        caso contrario es una nueva tupla
        */
        protected (string sql, List<object> param, string con) _sql_cond_recursive(List<object> condition)
        {
            
            if (condition[0].IsList()) //si el primer valor es una lista, corresponde a un arbol de condiciones
                return _sql_cond_iterable(condition);

            var field = (string)condition[0];
            var option = (string)condition[1];
            var value = condition[2];
            var con = (condition.Count == 4) ? (string)condition[3] : "AND";

            var condition_ = _sql_cond_field_check_value(field, option, value);
            return (condition_.sql, condition_.param, con);
        }

        /*
        Metodo iterable para definir condicion

        Se utiliza cuando la condicion esta formada por condiciones, hay que iterar sobre ellas
        */
        protected (string sql, List<object>? param, string con) _sql_cond_iterable(List<object> condition_iterable)
        {
            /*
            Lista de tuplas
            ({"sql":..., "params":..., "con":...}, {"sql":..., "params":..., "con":...}, ...)
            */
            List<(string sql, List<object>? param, string con)> conditions_conc = new();

            foreach (var ci in condition_iterable)
            {
                var cc = _sql_cond_recursive((List<object>)ci);
                conditions_conc.Add(cc);
            }

            string sql = "";
            List<object> param = new List<object>();
            bool first = true;

            foreach (var cc in conditions_conc)
            {
                if (first)
                {
                    sql += @"
" + cc.con + @"
";
                    first = false;
                }
                sql += cc.sql;

                if (!cc.param.IsNullOrEmpty()) param.AddRange(cc.param);
            }

            sql = @"(
" + sql + @"
)";

            return (sql, param, conditions_conc[0].con);
        }

        /*
        Combinar parametros y definir SQL
        */
        protected (string sql, List<object> param) _sql_cond_field_check_value(string field, string option, object value)
        {
            if (!value.IsList())
                return _sql_cond_field(field, option, value);

            string sql = "";
            List<object> param = new();
            bool include_con = false; //flag para indicar que debe incluirse la condicion
        
            foreach(var v in (List<object>)value)
            {
                if (include_con)
                    if (option == "equal" || option == "approx")
                        sql += " OR ";
                    else if (option == "nonequal" || option == "nonapprox")
                        sql += " AND ";
                    else
                        throw new Exception("Error al definir condicion " + field + " " + option + " " + value);
                else
                    include_con = true;

                var condition_ = _sql_cond_field_check_value(field, option, v);
                sql += condition_.sql;
                param.AddRange(condition_.param);
            }

            sql = @"(
" + sql + @"
)
";
            return (sql, param);
        }

        /*
        Traducir campo y definir SQL con la opcion
        */
        protected (string sql, List<object>? param) _sql_cond_field(string field_name, string option, object value)
        {
            var f = db.explode_field(entity_name, field_name);

            if (option.StartsWith("$")) //condicion entre fields
            {
                var v = db.explode_field(entity_name, (string)value);
                var field_sql1 = db.mapping(f["entity_name"], f["field_id"]).map(f["field_name"]);
                var field_sql2 = db.mapping(v["entity_name"], v["field_id"]).map(v["field_name"]);

                if (option == "approx")
                    return (
                        "(lower(CAST(" + field_sql1 + " AS CHAR)) LIKE CONCAT('%', lower(CAST(" + field_sql2 + " AS CHAR)), '%'))",
                        new List<object>()
                    );
                else if(option == "nonapprox")
                {
                    return (
                        "(lower(CAST(" + field_sql1 + " AS CHAR)) NOT LIKE CONCAT('%', lower(CAST(" + field_sql2 + " AS CHAR)), '%'))",
                        new List<object>()
                    );
                }
                else
                {
                    return (
                        "(" + field_sql1 + " " + option + " " + field_sql2 + ") ",
                        new List<object>()
                    );
                }               
            }

            return db.condition(f["entity_name"], f["field_id"]).cond(f["field_name"], option, value);
        }



    }

}