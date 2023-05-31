using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public class EntityTree
    {
        public string field_name { get; set; }
        public string entity_name { get; set; }
        public Dictionary<string, EntityTree> children { get; set; }
    }
}
