using Microsoft.VisualBasic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace cs_sqlo
{
    /*
    Definir condicion a traves de 3 elementos "field, option y value" donde 
    value es un valor válido para el field

    Ejemplo de subclase opcional:

    -class ComisionCondition(Condition):
        def numero(self):
           return '''
    some sql condition
'''

    Las subclases deben soportar la sintaxis del motor que se encuentran 
    utilizando.
    */
    public abstract class Condition : EntityOptions
    {
        public Condition(Db _db, string _entity_name, string _field_id) : base(_db, _entity_name, _field_id)
        {
        }

        /*
        Definir condicion

        Verificar la existencia de metodo exclusivo, si no exite, buscar metodo
        predefinido.

        Ejemplo: 
        -condition.cond("nombre", APPROX, "something")
        -condition.cond("fecha_alta.max.y", EQUAL, "2000"); //aplicar max, luego y
        */
        public (string sql, List<object> param) cond(string field_name, string option, object value)
        {
            var method = field_name.Replace(".", "_");
            Type thisType = this.GetType();
            MethodInfo m = thisType.GetMethod(method);
            if (!m.IsNullOrEmpty())
            {
                var r = m!.Invoke(this, null);
                return ((string sql, List<object> param))r!;
            }

            method = _define_condition_method(field_name);
            return ((string sql, List<object> param))thisType.GetMethod(method).Invoke(this, new object[3] { field_name, option, value });
        }

        /**
         * mapeo por defecto
         */
        public string _define_condition_method(string field_name)
        {
            List<string> p = field_name.Split(".").ToList();
            if (p.Count == 1)
            {
                Field field = db.field(entity_name, field_name);
                switch (field.type)
                { 
                    default:
                        return "_default";
                }
            }

            string method = "_" + p[p.Count - 1]; //se traduce el metodo ubicado mas a la derecha (el primero en traducirse se ejecutara al final)
            p.RemoveAt(p.Count - 1);

            switch (method){
                case "count": case "avg": case "sum":
                    return "_default";

                case "is_set": case "exists":
                    return "_exists";

                case "y":
                    return "_str";

                default:
                    return _define_condition_method(String.Join(".", p.ToArray())); //si no resuelve, intenta nuevamente (ejemplo field.count.max, intentara nuevamente con field.count)
            }   
        }

    }
}