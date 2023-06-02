using cs_sqlo;
    using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;
using System;

namespace cs_sqlo_ss
{
    public class EntityQuerySs : EntityQuery
    {

        public EntityQuerySs(Db db, string entity_name) : base(db, entity_name)
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

        Return tuple, example:
        {
            "sql": "nombres LIKE %s"
            "param": ("valores de variables",)
            "con": "AND"
        }
        */
        protected (string sql, List<object>? param, string con) _sql_cond_recursive(List<object> condition)
        {
            
            if (condition[0].IsList()) //si el primer valor es una lista, corresponde a un arbol de condiciones
                return _sql_cond_iterable(condition);

            var field = condition[0];
            var option = condition[1];
            var value = condition[2];
            var con = (condition.Count == 4) ? condition[3] : "AND";

            var condition_ = _sql_cond_field_check_value(field, option, value);
            return (condition_.field, condition_.param, con);
        }

        /*
        Si  la posicion 0 de condition_iterable es un string significa que es un campo a buscar

        Return tuple, example:
        {
            "sql": "nombres LIKE %s"
            "param": ("valores de variables",)
            "con": "AND"
        }
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
        protected (string sql, List<object>? param) _sql_cond_field_check_value(string field, string option, object value)
        {
            if (!value.IsList())
            {
                return _sql_cond_field(field, option, value);
            }

            string sql = "";
            List<object> param = new();
            bool include_cond = false; //flag para indicar que debe incluirse la condicion
        
            foreach(var v in (List<object>)value)
            {
                if (include_cond)
                {
                    string sql_ = (option == "equal" || option == "approx") ? " OR " :  
                }
            }
        }

        protected (string sql, List<object>? param) _sql_cond_field(string field, string option, object value)
        {
            throw new NotImplementedException();
        }


        

        condition = {
            "sql":"",
            "params":()
        }
        cond = False #flag para indicar que debe imprimirse condicion

        for v in value:
            if cond:
                sql = " {} ".format(OPTIONS["OR"]) if option == "EQUAL" or option == "APPROX" else " {} ".format(OPTIONS["AND"]) if option == "NONEQUAL" or option == "NONAPPROX" else False
                if not sql:
                    raise "Error al definir opción para " + field + " " + option + " " + value
                condition["sql"] += sql

            else:
                cond = True

            condition_ = self._sql_cond_field_check_value(field, option, v)
            condition["sql"] += condition_["sql"]
            condition["params"] = condition["params"] + condition_["params"]

        return {
            "sql":"""(
""" + condition["sql"] + """
)""",
            "params":condition["params"]
    }

}
}