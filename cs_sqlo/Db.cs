namespace cs_sqlo
{
    public class Db
    {
        protected Dictionary<string, object>? _config;
        protected Dictionary<string, object> _fields_config = new();
        protected Dictionary<string, object> _entity = new();
        protected Dictionary<string, object> _tools = new();
        protected Dictionary<string, object> _field = new();
        protected Dictionary<string, object> _mapping = new();
        protected Dictionary<string, object> _condition = new();    
    }


}