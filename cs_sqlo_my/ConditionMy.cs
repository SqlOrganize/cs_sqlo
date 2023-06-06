
namespace cs_sqlo
{
    /*
    Mapear campos para que sean entendidos por el motor de base de datos.

    Define SQL, cada motor debe tener su propia clase Condition de forma tal que
    sea traducido de forma correcta.

    Ejemplo de subclase opcional:

    -class ComisionCondition: Condition:
        def numero(self):
           return '''
    CONCAT("+self.pf()+"sed.numero, "+self.pt()+".division)
'''

    Las subclases deben soportar la sintaxis del motor que se encuentran utilizando.
    */
    public class ConditionMy : Condition
    {
        public ConditionMy(Db _db, string _entity_name, string _field_id) : base(_db, _entity_name, _field_id)
        {
        }

        public (string sql, List<object> param) _default(string field_name, string option, object value) {
            var field = db.mapping(entity_name, field_id).map(field_name);

            if (value is bool)
                return _exists(field, option, value);

            if ((option == "approx") || (option == "nonapprox"))
                return _approx_cast(field, option, value);

            return (
                    "(" + field + " " + Db.options[option] + " %s) ",
                    new List<object>() { _value(field_name, value) }
            );

        }

        public (string sql, List<object> param) _exists(string field_name, string option, object value) {
            if (option != "equal" && option != "nonequal") {
                throw new Exception("La combinacion field-option-value no está permitida para definir existencia: " + field_name + " " + option + " " + value);
            }

            if (((bool)value && option != "equal") || (!(bool)value && option != "nonequal"))
            {
                return (
                    "(" + field_name + " IS NOT NULL) ",
                    new List<object>()
                );
            } else {
                return (
                    "(" + field_name + " IS NULL) ",
                    new List<object>()
                );
            }
        }

        public (string sql, List<object> param) _approx_cast(string field_name, string option, object value) {

            if (option == "nonapprox")
                return (
                    "(LOWER(CAST(" + field_name + " AS CHAR)) NOT LIKE LOWER(%s)) ",
                    new List<Object>() { "%" + (string)value + "%" }
                );

            return (
                "(LOWER(CAST(" + field_name + " AS CHAR)) LIKE LOWER(%s)) ",
                new List<Object>() { "%" + (string)value + "%" }
            );

        }

        public object _value(string field_name, object value) {
            Values v = db.values(entity_name, field_id);

            v.sset(field_name, value);

            if (!v.check(field_name))
            {
                throw new Exception("Valor incorrecto al definir condicion: " + entity_name + " " + field_name + "  " + value);
            }

            return v.get(field_name); //no se aplica formato sql, el formato lo hace la clase de acceso a la base de datos
        }
zZ
    }
}