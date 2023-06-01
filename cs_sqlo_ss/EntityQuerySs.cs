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


        protected (string sql, List<object>? param, string con) _sql_cond_recursive(List<object> condition)
        {
            
            if (condition[0].IsList()) //si el primer valor es una lista, corresponde a un arbol de condiciones
                return _sql_cond_iterable(condition);

            var field = condition[0];
            var option = condition[1];
            var value = condition[2];
            var con = (condition.Count == 4) ? condition[3] : "AND";
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
        protected (string sql, List<object>? param, string con) _sql_cond_recursive(Cond condition)
        {
            if (condition.IsNullOrEmpty())
                throw new Exception("Condicion vacia");

            if (condition.HasTree()) //si el primer valor es una lista, corresponde a un arbol de condiciones
                return _sql_cond_iterable(condition);


            return _sql_cond_field_check_value(condition)
        }

        /*
        Metodo iterable para definir condicion

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


            }


            for ci in condition_iterable:
            cc = self._sql_cond_recursive(ci)
            conditions_conc = conditions_conc + (cc, )


        ret = {
            "sql": "",
            "params": (),
        }

        for cc in conditions_conc:
            if ret["sql"]:
                ret["sql"] += """
""" + cc["con"] + " "

            ret["sql"] += cc["sql"]
            ret["params"] = ret["params"] + cc["params"]
            
        return {
            "sql": """(
""" + ret["sql"] + """
)""", 
            "params":ret["params"],
            "con": conditions_conc[0]["con"] #primera condicion de la iteracion
        }

    }
}