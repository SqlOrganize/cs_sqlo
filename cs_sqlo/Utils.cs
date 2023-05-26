using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public static class Utils
    {
        public static Dictionary<K, V> merge_dicts<K, V>(IEnumerable<Dictionary<K, V>> dictionaries)
        {
            Dictionary<K, V> result = new Dictionary<K, V>();

            foreach (Dictionary<K, V> dict in dictionaries)
            {
                dict.ToList().ForEach(pair => result[pair.Key] = pair.Value);
            }

            return result;
        }
    }
}
