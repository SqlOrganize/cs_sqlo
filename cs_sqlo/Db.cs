using Newtonsoft.Json;
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
        protected Dictionary<string, Entity> _entity = new();
        protected Dictionary<string, object> _tools = new();
        protected Dictionary<string, Dictionary<string, Field>> _field = new();
        protected Dictionary<string, object> _mapping = new();
        protected Dictionary<string, object> _condition = new();

        /*
        { entity_name > { field_id > { keystring > value }}}
        */
        protected Dictionary<string, Dictionary<string, Dictionary<string, object>>> _tree = new();

        /*
        { entity_name > { field_id > { keystring > value }}}
        */
        protected Dictionary<string, Dictionary<string, Dictionary<string, object>>> _relations = new();

        /*
        { entity_name > { keystring > value }}
        */
        protected Dictionary<string, Dictionary<string, object>> _entities = new();
        
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
                _tree = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(r.ReadToEnd())!;
            }

            using (StreamReader r = new StreamReader(_config["path_model"] + "entity-relations.json"))
            {
                _relations = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, object>>>>(r.ReadToEnd())!;
            }

            using (StreamReader r = new StreamReader(_config["path_model"] + "_entities.json"))
            {
                _entities = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(r.ReadToEnd())!;
            }

            if (File.Exists(_config["path_model"] + "entities.json"))
            {
                using (StreamReader r = new StreamReader(_config["path_model"] + "entities.json"))
                {
                    Dictionary<string, Dictionary<string, object>> ee = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(r.ReadToEnd());
                    foreach (KeyValuePair<string, Dictionary<string, object>> e in ee)
                    {
                        if (_entities.ContainsKey(e.Key))
                        {
                            List<Dictionary<string, object>> p = new();
                            p.Add(_entities[e.Key]);
                            p.Add(ee[e.Key]);
                            _entities[e.Key] = Utils.MergeDicts(p);
                        }
                    }
                }

            }
        }

        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> tree() => _tree;
        public Dictionary<string, Dictionary<string, object>> tree_entity(string entity_name) => _tree[entity_name];
        public Dictionary<string, Dictionary<string, Dictionary<string, object>>> relations() => _relations;
        public Dictionary<string, Dictionary<string, object>> relations_entity(string entity_name) => _relations[entity_name];
        public Dictionary<string, object> entities_entity(string entity_name) => _entities[entity_name];
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
                    { "entity_name", (string)relations_entity(entity_name)[f[0]]["entity_name"] },
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
            if (!_entity.ContainsKey(entity_name))
                _entity[entity_name] = new Entity(this, entity_name);

            return _entity[entity_name];
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
