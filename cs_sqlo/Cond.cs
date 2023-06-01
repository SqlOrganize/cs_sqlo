
namespace cs_sqlo
{
    public class Cond
    {
        /*
        Si el arbol de condiciones es distinto de null, las opciones no son tenidas en cuenta
        */
        protected Cond[]? _tree;
        public Cond[]? tree
        {
            get
            {
                if (!field.IsNullOrEmpty() && !_tree.IsNullOrEmpty())
                {
                    throw new Exception("Error al definir propiedades de Cond");
                }
                return _tree;
            }

            set
            {
                if (!field.IsNullOrEmpty() && value is not null)
                {
                    throw new Exception("Error al definir propiedades de Cond");
                }
                _tree = value;
            }
        }

        public string? _field;

        public string? field
        {
            get
            {
                if (!field.IsNullOrEmpty() && !_tree.IsNullOrEmpty())
                {
                    throw new Exception("Error al definir propiedades de Cond");
                }
                return _field;
            }

            set
            {
                if (!_tree.IsNullOrEmpty() && value is not null)
                {
                    throw new Exception("Error al definir propiedades de Cond");
                }
                _field = value;
            }
        }

        /*
        Las opciones deben ser tenidas en cuenta si el arbol de condiciones es null
        (se verifica principalmente la propiedad field
        */
        public string option { get; set; } = "EQUAL";
        public object? value { get; set; }
        public string con { get; set; } = "AND";


        public Cond(params Cond[] _tree)
        { 
            tree = _tree;
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

        public bool IsNullOrEmpty()
        {
            return (tree.IsNullOrEmpty() && field.IsNullOrEmpty());
        }

        public bool IsTree()
        {
            return (!tree.IsNullOrEmpty());
        }

        public bool IsCond()
        {
            return (!field.IsNullOrEmpty());
        }

    }
}
