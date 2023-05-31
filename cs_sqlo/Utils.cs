using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public static class Utils
    {

        public static void CopyValues<T>(T target, T source)
        {
            Type t = typeof(T);

            var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);

            foreach (var prop in properties)
            {
                var value = prop.GetValue(source, null);
                if (!value.IsNullOrEmpty())
                    prop.SetValue(target, value, null);
            }
        }

        public static Dictionary<K, V> MergeDicts<K, V>(IEnumerable<Dictionary<K, V>> dictionaries) where K : notnull
        {
            Dictionary<K, V> result = new Dictionary<K, V>();

            foreach (Dictionary<K, V> dict in dictionaries)
            {
                dict.ToList().ForEach(pair => result[pair.Key] = pair.Value);
            }

            return result;
        }

        public static bool IsNullOrEmpty(this IList List)
        {
            return (List == null || List.Count < 1);
        }

        public static bool IsNullOrEmpty(this IDictionary Dictionary)
        {
            return (Dictionary == null || Dictionary.Count < 1);
        }

        public static bool IsNullOrEmpty(this object? o)
        {
            if (o is IDictionary) return ((IDictionary)o).IsNullOrEmpty();
            if (o is IList) return ((IList)o).IsNullOrEmpty();
            if (o is string) return ((string)o).IsNullOrEmpty();
            return o == null;
        }

    }
       
}
