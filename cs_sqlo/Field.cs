using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    internal class Field
    {
        protected Db _db;
        protected string _name;

        /* nombre de la entidad */
        protected string _entity_name;

        /* 
        si es clave foranea: Nombre de la entidad referenciada por la clave foranea 
        */
        protected string? _entity_ref_name;

        /* 
         si es clave foranea: Nombre del campo referenciado (habitualmente "id")
         */
        protected string? _field_ref_name;
        protected string? _alias;

        /* 
        tipo de datos generico 
            int
            blob
            string
            boolean
            float
            text
            timestamp
            date               
         */
        protected string _type;


        /* 
        string con el tipo de field
            "pk": Clave primaria
            "nf": Field normal
            "mo": Clave foranea muchos a uno
            "oo": Clave foranea uno a uno
        */
        protected string _field_type;

        /* valor por defecto */
        protected object? _default;

        /* longitud maxima permitida */
        //protected int? _length;  

        /* valor maximo permitido */
        //protected object? _max;  

        /* valor minimo permitido */
        //protected object? _min;  

        /* lista de valores permitidos */
        //List<object> _values;

        public Field(Db db, string name, string entity_name, string field_name)
        {
            _db = db;
            _name = field_name;
            _entity_name = entity_name;

            public static void SubDictionaryUsingForEach(Dictionary<int, string> monthsInYear)
            {
                foreach (var month in monthsInYear)
                {
                    Console.WriteLine($"{month.Key}: {month.Value}");
                }
            }

            Dictionary<string, object> config = _db.fields_field(entity_name, field_name);


            foreach (KeyValuePair<string, object> c in config)
                if (c.Key.Contains("+"))
                {
                    string k = c.Key.TrimEnd(new Char[] { '+' });
                    object[]  = (object[])c.Value;
                }

            if "+" in k:
            k = k.rstrip("+")
                for vv in v:
                    if vv not in config[k]:
                        config[k].append(vv)
            elif "-" in k:
            k = k.rstrip("-")
                diff = [i for i in config[k] + v if i not in config[k] or i not in v]
                config[k] = diff

            setattr(self, "_" + k, config[k])
        }
     
    }
}
