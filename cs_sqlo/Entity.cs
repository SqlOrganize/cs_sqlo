using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public class Entity
    {
        protected Db _db;
        protected string _name;
        protected string _alias;
        protected string? _schema;

        protected List<string> _pk = new();
        protected List<string> _nf = new();
        protected List<string> _mo = new();
        protected List<string> _oo = new();

        /* 
        array dinamico para identificar univocamente a una entidad en un momento determinado
        @example
        identifier = ["fecha_anio", "fecha_semestre","persona-numero_documento"]
        */
        protected List<string> _identifier = new();

        /*
        Valores por defecto para ordenamiento
        @example ["field1"=>"asc","field2"=>"desc",...];
        */
        protected Dictionary<string, string> _order_default = new();

        /*
        Valores no administrables
        @example ["field1","field2",...]
        */
        protected List<string> _no_admin = new();

        /*
        Valores principales
        @example ["field1","field2",...]
        */
        protected List<string> _main = new() { "id" };

        /*
        Valores unicos
        Una entidad puede tener varios campos que determinen un valor unico
        @example ["field1","field2",...]
        */
        protected List<string> _unique = new() { "id" };

        /*
        Valores unicos multiples
        Solo puede especificarse un juego de campos unique_multiple
        */
        protected List<string> _unique_multiple = new();

        public Entity(Db db, string entity_name)
        {
            _db = db;
            _name = entity_name;
            Dictionary<string, object> config = _db.entities_entity(entity_name);
            this.SetConfig(config);
        }

        public string name() => _name;
        public string alias() => _alias;
        public string schema_() => String.IsNullOrEmpty(_schema) ? _schema : "";
        public string schema() => _schema;
        public string schema_name() => schema() + name();
        public string schema_name_alias() => schema() + name() + " AS " + alias();
        public List<string> identifier() => _identifier;

        protected List<Field> _fields(List<string> field_names)
        {
            List<Field> fields = new();
            foreach (string field_name in field_names)
            {
                fields.Add(_db.field(_name, field_name));
            }

            return fields;

        }

        /*
        fields no fk
        */
        public List<Field> nf() => _fields(_nf);

        /*
        fields many to one
        */
        public List<Field> mo() => _fields(_mo);

        /*
        fields one to one (local fk)
        */
        public List<Field> oo() => _fields(_oo);

        /*
        fields fk (mo + oo)
        */
        public List<Field> fk() => mo().Concat(oo()).ToList();

        /*
        all fields except pk"
        */
        public List<Field> fields_no_pk(List<string> field_names) => nf().Concat(mo()).ToList().Concat(oo()).ToList();

        public List<Field> fields(List<string> field_names) => nf().Concat(mo()).ToList().Concat(oo()).ToList();

        /*
        fields one to many
        its neccesary to iterate over all entities
        */
        public List<Field> om() {
            List<Field> fields = new();

            foreach (string entity_name in _db.entity_names())
            {
                Entity e = _db.entity(entity_name);
                foreach (Field f in e.mo())
                {
                    if (f.entity_ref().name() == this.name())
                    {
                        fields.Add(f);
                    }
                }
            }

            return fields;
        }

        /*
        fields one to one without local fk
        fk pointed to entity outside
        its neccesary to iterate over all entities
        */
        public List<Field> oon()
        {
            List<Field> fields = new();

            foreach (string entity_name in _db.entity_names())
            {
                Entity e = _db.entity(entity_name);
                foreach (Field f in e.oo())
                {
                    if (f.entity_ref().name() == this.name())
                    {
                        fields.Add(f);
                    }
                }
            }

            return fields;
        }

        /*
        fields referenced (om + oon)
        fk pointed to entity outside
        its neccesary to iterate over all entities
        */
        public List<Field> fref() => om().Concat(oon()).ToList();

        public Dictionary<string, string> order_default() => _order_default;

        public List<string> field_names() => _db.field_names(name());

        public List<string> unique() => _unique;

        public List<string> unique_multiple() => _unique_multiple;

        public List<string> main() => _main;
    }
}