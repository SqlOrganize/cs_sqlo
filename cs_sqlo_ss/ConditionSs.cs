using Newtonsoft.Json.Linq;

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
    public class ConditionSs : Condition
    {
        public ConditionSs(Db _db, string _entity_name, string _field_id) : base(_db, _entity_name, _field_id)
        {
        }

        public (string sql, List<object> param) _default(string field_name, string option, object value) {
            var field = db.mapping(entity_name, field_id).map(field_name);

            if (value is bool)
                return _exists(field, option, value);

            if ((option == "approx") or (option == "nonapprox"))
                return _approx_cast(field, option, value);

            return (
                    "(" + field + " " + db.options[option] + " %s) ",
                    _value(field_name, value)
            );

        }

        public (string sql, List<object> param) _exists(string field_name, string option, object value) {
            if option != "EQUAL" and option != "NONEQUAL":
                raise "La combinacion field-option-value no está permitida para definir existencia: " + field_name + " " + option + " " + value

            return {
                "sql":"(" + field_name + " IS NOT NULL) ",
                "params":()
            } if (value and option == "EQUAL") or (not value and option == "NONEQUAL") else {
                "sql":"(" + field_name + " IS NULL) ",
                "params":()
            }
        }

        public (string sql, List<object> param) _approx_cast(self, field_name, option, value) {
            if option == "APPROX": 
                return {
                "sql":"(LOWER(CAST(" + field_name + " AS CHAR)) LIKE LOWER(%s)) ",
                    "params":("%" + value + "%", )
                }

            if option == "NONAPPROX":
                return {
                "sql":"(LOWER(CAST(" + field_name + " AS CHAR)) NOT LIKE LOWER(%s)) ",
                    "params":("%" + value + "%", )
                }

            return { "sql":"", "params":() }
        }

        public object _value(string field_name, object value) {
            v = db.values(self._entity_name, self._prefix)

            v.sset(field_name, value)

            if not v.check(field_name):
                raise "Valor incorrecto al definir condicion: " + self._entity_name + " " + field_name + "  " + value


            return v.sql(field_name)
        }

    }
}