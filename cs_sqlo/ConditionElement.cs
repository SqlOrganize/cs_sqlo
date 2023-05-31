
namespace cs_sqlo
{
    public class ConditionElement
    {
        public string field { get; set; }
        public string option { get; set; }
        public object value { get; set; }
        public string con { get; set; }

        public ConditionElement(string _field, object _value, string _option = "EQUAL", string _con = "AND")
        { 
            field = _field;
            option = _option;   
            value = _value;
            con = _con;
        }

    }
}
