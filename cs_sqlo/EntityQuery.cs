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


        public EntityQuery w(string w)
        {
            where = w;
            return this;
        }

        public EntityQuery f(string f)
        {
            fields += f;
            return this;
        }

        public EntityQuery fa()
        {
            fields_as += string.Join(", ", db.tools(entity_name).field_names().Select(x => "$" + x));
            return this;
        }

        public EntityQuery fa(string f)
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
                Console.WriteLine(_sql[i]);

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
                var a = (f["field_id"].IsNullOrEmpty()) ? f["field_name"] : f["field_id"];
                ff += " AS '" + a + "'";
            }
           return ff;
        }

     
        public EntityQuery s(int _size)
        {
            size = _size;
            return this;
        }
        public EntityQuery p(int _page)
        {
            page = _page;
            return this;
        }

        public EntityQuery o(string _order)
        {
            order = _order;
            return this;
        }



        public EntityQuery h(string h)
        {
            having += h;
            return this;
        }

        public EntityQuery g(string g)
        {
            group += g;
            return this;
        }


        public string sql()
        {
            var sql = "SELECT ";
            sql += traduce(this.fields);
            sql += traduce(this.fields_as, true);
            sql += traduce(this.group);
            sql += from();
            sql += (where.IsNullOrEmpty()) ? "" : "WHERE " + traduce(where);
            sql += (group.IsNullOrEmpty()) ? "" : "GROUP BY " + traduce(group);

            return sql;
        }

        protected string from()
        {
            return @" FROM 

" + db.entity(entity_name).schema_name + @"
" + db.entity(entity_name).alias + @"
";
         }



    }

}