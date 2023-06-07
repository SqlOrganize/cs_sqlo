using Newtonsoft.Json.Linq;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace cs_sqlo
{
    public abstract class EntityQuery
    {
        public Db db { get; }
        public string entity_name { get; }


        /*
        Debe respetar el formato del motor de base de datos
        Los fields se traducen con los metodos de mapeo, deben indicarse con el prefijo $
        . indica aplicacion de funcion de agregacion
        - indica que pertenece a una relacion
        Ej "($ingreso = %p1) AND ($persona-nombres = %p1)"
        */
        public string? where { get; set; }

        /*
        Debe respetar el formato del motor de base de datos
        Los fields se traducen con los metodos de mapeo, deben indicarse con el prefijo $
        . indica aplicacion de funcion de agregacion
        - indica que pertenece a una relacion
        Ej "($ingreso = %p1) AND ($persona-nombres = %p1)"
        */
        public string? having { get; set; }

        /*
        Los fields deben estar definidos en el mapping field, se realizará la 
        traducción correspondiente
        Para traducir se les debe indicar el prefijo "$"
        . indica aplicacion de funcion de agregacion
        - indica que pertenece a una relacion
        Ej "$nombres, $curso-horas_catedra.sum" => Se traduce a "alum.nombres as 'nombres', SUM(cur.horas_catedra) AS 'curso-horas_catedra.sum'"
        */
        public string? fields { get; set; } = "";
        /*
        Similar a fields, pero no se aplica renombramiento

        "$nombres, $curso-horas_catedra.sum" => Se traduce a "alum.nombres, SUM(cur.horas_catedra)"
        */

        public string? fields_as { get; set; } = "";


        public string? order { get; set; } = "";
        public int? size { get; set; } = 100;
        public int? page { get; set; } = 1;

        /*
        Similar a fields pero campo de agrupamiento
        */
        protected string group { get; set; } = "";

        public EntityQuery(Db _db, string _entity_name)
        {
            db = _db;
            entity_name = _entity_name;
        }


        public EntityQuery Where(string w)
        {
            where = w;
            return this;
        }

        public EntityQuery Fields(string f)
        {
            fields += f;
            return this;
        }

        public EntityQuery FieldsAs()
        {
            fields_as += string.Join(", ", db.tools(entity_name).field_names().Select(x => "$" + x));
            return this;
        }

        public EntityQuery FieldsAs(string f)
        {
            fields_as += f;
            return this;
        }

        protected string traduce(string _sql, bool flag_as = false)
        {
            string sql = "";
            int field_start = -1;

            for (int i = 0; i < _sql.Length; i++)
            {
                if (_sql[i] == '$')
                {
                    field_start = i;
                    continue;
                }

                if (field_start != -1)
                {
                    if ((_sql[i] != ' ') && (_sql[i] != ')') && (_sql[i] != ',')) continue;
                    sql += traduce_(_sql, flag_as, field_start, i - field_start - 1);
                    field_start = -1;
                }

                sql += _sql[i];

            }

            if (field_start != -1)
            {
                sql += traduce_(_sql, flag_as, field_start, _sql.Length - field_start - 1);
            }


            return sql;
        }

        protected string traduce_(string _sql, bool flag_as, int field_start, int field_end)
        {
            var field_name = _sql.Substring(field_start + 1, field_end);
            var f = db.explode_field(entity_name, field_name);

            var ff = db.mapping(f["entity_name"], f["field_id"]).map(f["field_name"]);
            if (flag_as)
            {
                var a = (f["field_id"].IsNullOrEmpty()) ? f["field_name"] : field_name;
                ff += " AS '" + a + "'";
            }
            return ff;
        }


        public EntityQuery Size(int _size)
        {
            size = _size;
            return this;
        }
        public EntityQuery Page(int _page)
        {
            page = _page;
            return this;
        }

        public EntityQuery Order(string _order)
        {
            order = _order;
            return this;
        }



        public EntityQuery Having(string h)
        {
            having += h;
            return this;
        }

        public EntityQuery Group(string g)
        {
            group += g;
            return this;
        }

        protected string sql_join()
        {
            string sql = "";
            Dictionary<string, EntityTree> tree = db.tree[entity_name];
            sql += sql_join_fk(tree, "");
            return sql;
        }

        protected string sql_join_fk(Dictionary<string, EntityTree> tree, string table_id)
        {
            if (table_id.IsNullOrEmpty())
                table_id = db.entity(entity_name).alias;

            string sql = "";
            string schema_name;
            foreach (var (field_id, entity_tree) in tree) {
                schema_name = db.entity(entity_tree.entity_name).schema_name;
                sql += "LEFT OUTER JOIN " + schema_name + " AS " + field_id + " ON (" + table_id + "." + entity_tree.field_name + " = " + field_id + "." + entity_tree.field_ref_name + @")
";

                if (!entity_tree.children.IsNullOrEmpty()) sql += sql_join_fk(entity_tree.children, field_id);
            }
            return sql;
        }

        public string Sql()
        {
            if (order.IsNullOrEmpty()) order = string.Join(", ", db.entity(entity_name).order_default.Select(x => "$" + x));
            

            var sql = "SELECT ";
            sql += sql_fields();
            sql += sql_from();
            sql += sql_join();
            sql += (where.IsNullOrEmpty()) ? "" : "WHERE " + traduce(where) + @"
";
            sql += (group.IsNullOrEmpty()) ? "" : "GROUP BY " + traduce(group) + @"
";
            sql += (having.IsNullOrEmpty()) ? "" : "HAVING " + traduce(having) + @"
";
            sql += (order.IsNullOrEmpty()) ? "" : "ORDER BY " + traduce(order) + @"
";
            sql += limit();

            return sql;
        }

        protected string sql_fields()
        {
            string f = concat(traduce(this.fields), @"
");
            f += concat(traduce(this.fields_as, true), @",
", "", !f.IsNullOrEmpty());

            f += concat(traduce(this.group), @",
", "", !f.IsNullOrEmpty());

            return f;
        }

        protected string sql_from()
        {
            return @" FROM 
" + db.entity(entity_name).schema_name + " AS " + db.entity(entity_name).alias + @"
";
        }

        protected  string limit()
        {
            if (!size.IsNullOrEmpty()) return "";
            page = page.IsNullOrEmpty() ? 1 : page;
            return " LIMIT " + size  + " OFFSET " + ((page - 1) * size ) + @"
";
        }

        protected string concat(string? value, string connect_no_empty, string connect_empty = "", bool connect_cond = false)
        {
            if (value.IsNullOrEmpty()) return "";

            string connect = "";
            if (!connect_empty.IsNullOrEmpty())
                connect = connect_cond ? connect_no_empty! : connect_empty;
            else
                connect = connect_no_empty;

            return connect + " " + value;
        }


    }

}