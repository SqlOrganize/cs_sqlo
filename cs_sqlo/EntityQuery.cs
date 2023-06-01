﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public class EntityQuery
    {
        public Db db { get; }
        public string entity_name { get; }

        /*
        condicion
        array multiple cuya raiz es [field,option,value], ejemplo: [["nombre","=","unNombre"],[["apellido","=","unApellido"],["apellido","=","otroApellido","OR"]]]
        */
        public List<object> _condition = new();

        public Dictionary<string, string> _order = new();
        public int _size = 100;
        public int _page = 1;

        /*
        Los fields deben estar definidos en el mapping field, se realizará la 
        traducción correspondiente
        . indica aplicacion de funcion de agregacion
        - indica que pertenece a una relacion
        Ej ["nombres", "horas_catedra.sum", "edad.avg", "com_cur-horas_catedra]
        */
        public List<string> _fields = new();

        /*
        Similar a _fields pero se define un alias para concatenar un conjunto de fields
        Ej ["nombre" => ["nombres", "apellidos"], "max" => ["horas_catedra.max", "edad.max"]]
        */
        public Dictionary<string, List<string>> _fields_concat = new();

        /*
        Similar a fields pero campo de agrupamiento
        */
        public List<string> _group = new();

        /*
        Similar a _fields_concat pero campo de agrupamiento
        */
        public Dictionary<string, List<string>> _group_concat = new();

        /*
        condicion de agrupamiento
        array multiple cuya raiz es [field,option,value], ejemplo: [["nombre","=","unNombre"],[["apellido","=","unApellido"],["apellido","=","otroApellido","OR"]]]
        */
        public List<object> _having = new();

        /*
        Campos a los cuales se aplica str_agg

        Array multiple definido por alias y los campos que se aplica str_agg

        @EXAMPLE

        -{"alias" => ["field1","field2"]} 
        
        @TRADUCCION DEL EJEMPLO

        -GROUP_CONCAT(DISTINCT field1_map, " ", field2_map) AS "alias"

        @STR_AGG A UN SOLO VALOR

        Para aplicar GROUP_CONCAT a un solo valor, se puede utilizar como al-
        ternativa la funcion de agreacion, por ejemplo persona.str_agg que se
        traduce a:        
        
        -GROUP_CONCAT(DISTINCT persona)
        */

        public Dictionary<string, List<string>> _str_agg = new();


        public EntityQuery(Db _db, string _entity_name)
        {
            db = _db;
            entity_name = _entity_name;
        }

        public EntityQuery cond(List<object> c)
        {
            _condition.Add(c);
            return this;
        }

        public EntityQuery param(string key, object value)
        {
            cond(new List<object>() { key, "EQUAL", value });
            return this;
        }

        public EntityQuery param_(Dictionary<string, object> param_)
        {
            foreach (var p in param_)
                cond(new List<object>() { p.Key, "EQUAL", p.Value });
            return this;
        }

        public EntityQuery order(Dictionary<string, string> order)
        {
            _order = order;
            return this;
        }
        public EntityQuery size(int size)
        {
            _size = size;
            return this;
        }
        public EntityQuery page(int page)
        {
            _size = page;
            return this;
        }

        public EntityQuery field(string field)
        {
            _fields.Add(field);
            return this;
        }
        public EntityQuery fields(List<string>? fields)
        {
            if (fields.IsNullOrEmpty())
            {
                return fields_tree();
            }
            _fields = _fields.Union(fields).ToList();
            return this;
        }

        public EntityQuery fields_tree()
        {
            //_fields = db.tools(entity_name).field_names();
            return this;
        }
        public EntityQuery fields_concat(Dictionary<string, List<string>> fields)
        {
            _fields_concat = Utils.MergeDicts(_fields_concat, fields);
            return this;
        }

        public EntityQuery group(List<string> group)
        {
            _group = group.Union(group).ToList();

            return this;
        }

        public EntityQuery group_concat(Dictionary<string, List<string>> group)
        {
            _group_concat = Utils.MergeDicts(_group_concat, group);
            return this;
        }
        public EntityQuery str_agg(Dictionary<string, List<string>> fields)
        {
            _str_agg = Utils.MergeDicts(_str_agg, fields);
            return this;
        }
        public EntityQuery hav(List<string> having)
        {
            _having = _having.Union(having).ToList();
            return this;
        }

        /*
        definir condicion para campos unicos 

        # ejemplo param
        {"field_name":"field_value", ...}
        
        # campos unicos simples
        Se definen a traves del atributo Entity.unique
        
        # campos unicos multiples
        Se definen a traves del atributo Entity.unique_multiple
        */
        public EntityQuery unique(Dictionary<string, string> param)
        {
            List<string> unique_fields = db.entity(entity_name).unique;
            List<string> unique_fields_multiple = db.entity(entity_name).unique_multiple;

            List<object> condition = new();

            bool first = true;

            foreach (var field_name in unique_fields)
            {
                foreach (var p in param)
                {
                    if ((p.Key == field_name) && !p.Value.IsNullOrEmpty())
                    {
                        string con = "";
                        if (first)
                        {
                            con = "AND";
                            first = false;
                        }
                        else
                        {
                            con = "OR";
                        }
                        condition.Add(new List<object>() { p.Key, "EQUAL", p.Value, con });
                    }
                }

            }

            if (!unique_fields_multiple.IsNullOrEmpty())
            {
                List<object> condition_multiple = new();
                first = true;
                bool exists_condition_multiple = true; //si algun campo de la condicion multiple no se encuentra definido, se carga en true.
                foreach (var f in unique_fields_multiple)
                {
                    if (!exists_condition_multiple) break;

                    exists_condition_multiple = false;

                    foreach (var p in param)
                    {
                        if (p.Key == f)
                        {
                            string con = "";
                            exists_condition_multiple = true;
                            if (first && !condition.IsNullOrEmpty())
                            {
                                con = "OR";
                                first = false;
                            }
                            else
                            {
                                con = "AND";
                            }
                            condition_multiple.Add(new List<object>() { p.Key, "EQUAL", p.Value, con });
                        }
                    }
                }
                if (exists_condition_multiple && !condition_multiple.IsNullOrEmpty())
                {
                    condition.Add(condition_multiple);
                }

            }

            return this;
        }
    } 
}