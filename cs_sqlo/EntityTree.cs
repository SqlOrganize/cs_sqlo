
namespace cs_sqlo
{
    /*
    Lectura de json
    */
    public class EntityTree
    {
        public string field_name { get; set; }
        public string entity_name { get; set; }
        public Dictionary<string, EntityTree> children { get; set; }
    }
}
