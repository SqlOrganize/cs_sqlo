
namespace cs_sqlo
{
    /*
deprecated?
    para lenguajes fuertemente tipados, el uso de json o juego de arrays puede resultar complejo
    se define una clase Condition Element (Cond) para facilitar la manipulacion, luego
    */
    public class Cond
    {
        public List<Cond> tree { get; set; }  = new();
        public string? field { get; set; }
        public string option { get; set; } = "EQUAL";
        public object? value { get; set; }
        public string con { get; set; } = "AND";

        public Cond()
        {
        }
        
        public Cond(params Cond[] _tree)
        {
            Add(_tree);
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

        public bool IsNullOrEmpty()
        {
            return (tree.IsNullOrEmpty() && field.IsNullOrEmpty());
        }

        public bool HasTree()
        {
            return (!tree.IsNullOrEmpty());
        }

        public bool HasField()
        {
            return (!field.IsNullOrEmpty());
        }

        public void Add(params Cond[] _tree)
        {
            tree.AddRange(_tree.ToList());
        }

    }
}
