using Newtonsoft.Json;
using System.Text.Json;

using System.IO;
using System.Text;

namespace cs_sqlo
{
    public class Db
    {
        public Dictionary<string, string> options = new()
        {
            {"EQUAL","="},
            {"NONEQUAL","!="},
            {"UNDEFINED","~"},
            {"DEFAULT","DEFAULT"},
            {"NONAPPROX","NONAPPROX"}, //comparacion apriximadamente distinto
            {"APPROX","APPROX"},
            {"APPROX_LEFT","APPROX_LEFT"},
            {"APPROX_RIGHT","APPROX_RIGHT"},
            {"AND","AND"},
            {"OR","OR"},
            {"$","$"}, //prefijo que indica field (utilizado para indicar concatenacion AND en condiciones)
            {"LESS","<"},
            {"LESS_EQUAL","<="},
            {"GREATER",">"},
            {"GREATER_EQUAL",">="},
        };
    
        protected Dictionary<string, object>? _config;
        protected Dictionary<string, object> _tools = new();
        protected Dictionary<string, Dictionary<string, Field>> _field = new();
        protected Dictionary<string, object> _mapping = new();
        protected Dictionary<string, object> _condition = new();

        /*
        { entity_name > { field_id > { keystring > value }}}
        */
        protected Dictionary<string, Dictionary<string, EntityTree>> _tree = new();

        /*
        { entity_name > { field_id > { keystring > value }}}
        */
        protected Dictionary<string, Dictionary<string, EntityRel>> _relations = new();

        /*
        { entity_name > { keystring > value }}
        */
        protected Dictionary<string, Entity> entities { get; set; }

        /*
        { entity_name > { field_name > { keystring > value }}}
        */
        protected Dictionary<string, Dictionary<string, Dictionary<string, object>>> _fields = new();

        public Db(Dictionary<string, object> config)
        {
            _config = config;
            string path = _config["path_model"] + "entity-tree.json";
            using (StreamReader r = new StreamReader(path, Encoding.UTF8))
            {
                 _tree = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, EntityTree>>>(r.ReadToEnd())!;
            }

            using (StreamReader r = new StreamReader(_config["path_model"] + "entity-relations.json"))
            {
                _relations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, EntityRel>>>(r.ReadToEnd())!;
            }

            using (StreamReader r = new StreamReader(_config["path_model"] + "_entities.json"))
            {
                entities = JsonConvert.DeserializeObject<Dictionary<string, Entity>>(r.ReadToEnd())!;
            }

            if (File.Exists(_config["path_model"] + "entities.json"))
            {
                using (StreamReader r = new StreamReader(_config["path_model"] + "entities.json"))
                {
                    Dictionary<string, Entity> ee = JsonConvert.DeserializeObject<Dictionary<string, Entity>>(r.ReadToEnd());
                    foreach (KeyValuePair<string, Entity> e in ee)
                    {
                        if (entities.ContainsKey(e.Key))
                        {
                            Utils.CopyValues(entities[e.Key], e.Value);                            
                        }
                    }
                }
            }

            foreach(KeyValuePair<string, Entity> e in entities)
            {
                if(!e.Value.nf_add.IsNullOrEmpty())
                {
                    var nf = new List<string>(e.Value.nf.Count + e.Value.nf_add.Count);
                    nf.AddRange(e.Value.nf);
                    nf.AddRange(e.Value.nf_add);
                    e.Value.nf = nf;
                    e.Value.nf_add.Clear();
                }
                if (!e.Value.mo_add.IsNullOrEmpty())
                {
                    var mo = new List<string>(e.Value.mo.Count + e.Value.mo_add.Count);
                    mo.AddRange(e.Value.mo);
                    mo.AddRange(e.Value.mo_add);
                    e.Value.mo = mo;
                    e.Value.mo_add.Clear();
                }
                if (!e.Value.oo_add.IsNullOrEmpty())
                {
                    var oo = new List<string>(e.Value.oo.Count + e.Value.oo_add.Count);
                    oo.AddRange(e.Value.oo);
                    oo.AddRange(e.Value.oo_add);
                    e.Value.oo = oo;
                    e.Value.oo_add.Clear();
                }
                if (!e.Value.unique_add.IsNullOrEmpty())
                {
                    var unique = new List<string>(e.Value.unique.Count + e.Value.unique_add.Count);
                    unique.AddRange(e.Value.unique);
                    unique.AddRange(e.Value.unique_add);
                    e.Value.unique = unique;
                    e.Value.unique_add.Clear();
                }
                if (!e.Value.not_null_add.IsNullOrEmpty())
                {
                    var not_null = new List<string>(e.Value.not_null.Count + e.Value.not_null_add.Count);
                    not_null.AddRange(e.Value.not_null);
                    not_null.AddRange(e.Value.not_null_add);
                    e.Value.not_null = not_null;
                    e.Value.not_null_add.Clear();
                }
                if (!e.Value.nf_sub.IsNullOrEmpty())
                {
                    e.Value.nf = e.Value.nf.Except(e.Value.nf_sub).ToList();
                    e.Value.nf_sub.Clear();
                }
                if (!e.Value.mo_sub.IsNullOrEmpty())
                {
                    e.Value.mo = e.Value.mo.Except(e.Value.mo_sub).ToList();
                    e.Value.mo_sub.Clear();
                }
                if (!e.Value.oo_sub.IsNullOrEmpty())
                {
                    e.Value.oo = e.Value.oo.Except(e.Value.oo_sub).ToList();
                    e.Value.oo_sub.Clear();
                }
                if (!e.Value.unique_sub.IsNullOrEmpty())
                {
                    e.Value.unique = e.Value.unique.Except(e.Value.unique_sub).ToList();
                    e.Value.unique_sub.Clear();
                }
                if (!e.Value.not_null_sub.IsNullOrEmpty())
                {
                    e.Value.not_null = e.Value.not_null.Except(e.Value.not_null_sub).ToList();
                    e.Value.not_null_sub.Clear();
                }
            }
        }

        public Dictionary<string, Dictionary<string, EntityTree>> tree() => _tree;
        public Dictionary<string, EntityTree> tree_entity(string entity_name) => _tree[entity_name];
        public Dictionary<string, Dictionary<string, EntityRel>> relations() => _relations;
        public Dictionary<string, EntityRel> relations_entity(string entity_name) => _relations[entity_name];
        public Dictionary<string, Dictionary<string, object>> fields_entity(string entity_name)
        {
            if (!_fields.ContainsKey(entity_name))
            {
                using (StreamReader r = new StreamReader(_config!["path_model"] + "fields/_"+entity_name+".json"))
                {
                    _fields[entity_name] = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(r.ReadToEnd())!;

                    if (File.Exists(_config["path_model"] + "fields/" + entity_name + ".json"))
                    {
                        using (StreamReader r2 = new StreamReader(_config["path_model"] + "fields/" + entity_name + ".json"))
                        {
                            Dictionary<string, Dictionary<string, object>> ee = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(r2.ReadToEnd());
                            foreach (KeyValuePair<string, Dictionary<string, object>> e in ee) 
                            {
                                if (_fields[entity_name].ContainsKey(e.Key))
                                {
                                    List<Dictionary<string, object>> p = new();
                                    p.Add(_fields[entity_name][e.Key]);
                                    p.Add(ee[e.Key]);
                                    _fields[entity_name][e.Key] = Utils.MergeDicts(p);
                                }
                            }
                        }

                    }

                }
            }

            return _fields[entity_name];
        }

        /* 
        configuracion de field

        Si no existe el field consultado se devuelve una configuracion vacia
        No es obligatorio que exista el field en la configuracion, se cargaran los parametros por defecto.
        */
        public Dictionary<string, object> fields_field(string entity_name, string field_name)
        {
            Dictionary<string, Dictionary<string, object>> fe = fields_entity(entity_name);
            return (fe.ContainsKey(field_name)) ? fe[entity_name] : new Dictionary<string, object>();
        }

        public List<string> entity_names() => _tree.Keys.ToList();
        public List<string> field_names(string entity_name) => fields_entity(entity_name).Keys.ToList();
        public Dictionary<string, string> explode_field(string entity_name, string field_name)
        {
            List<string> f = field_name.Split("-").ToList();

            if (f.Count() == 2)
            {
                return new Dictionary<string, string>
                {
                    { "field_id", f[0] },
                    { "entity_name", relations_entity(entity_name)[f[0]].entity_name },
                    { "field_name", f[0] },
                };

            }

            return new Dictionary<string, string>
            {
                { "field_id", "" },
                { "entity_name", entity_name },
                { "field_name", field_name },
            };
        }

        public Entity entity(string entity_name)
        {
            return entities[entity_name];
        }

        public Field field(string entity_name, string field_name)
        {
            if (!_field.ContainsKey(entity_name))
                _field[entity_name] = new();

            if (!_field[entity_name].ContainsKey(field_name))
                _field[entity_name][field_name] = new Field(this, entity_name, field_name);

            return _field[entity_name][field_name];
        }

        //field_by_id(self, entity_name:str, field_id:str) 
        //tools(self, entity_name)
        //mapping(self, entity_name: str, field_id:str = "")
        //condition(self, entity_name: str, field_id:str = "")
        //query(self, entity_name)
        //values(self, entity_name: str, field_id:str = "")

    }

}
