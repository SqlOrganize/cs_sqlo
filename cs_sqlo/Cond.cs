
namespace cs_sqlo
{
    public class Cond
    {
        public Cond[]? ce;
        public string? field { get; set; }
        public string option { get; set; } = "EQUAL";
        public object? value { get; set; }
        public string con { get; set; } = "AND";


        public Cond(params Cond[] _ce)
        { 
            ce = _ce;
        }

        public Cond(string _field, string _option, object _value, string _con)
        {
            field = _field;
            option = _option;
            value = _value;
            con = _con;
        }

        public Cond(string _field, object _value)
        {
            field = _field;
            value = _value;
        }

        public Cond(string _field, object _value, string _con)
        {
            field = _field;
            value = _value;
            con = _con;
        }

    }
}
