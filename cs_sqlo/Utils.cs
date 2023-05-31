using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cs_sqlo
{
    public static class Utils
    {
        public static Dictionary<K, V> MergeDicts<K, V>(IEnumerable<Dictionary<K, V>> dictionaries) where K : notnull    
        {
            Dictionary<K, V> result = new Dictionary<K, V>();

            foreach (Dictionary<K, V> dict in dictionaries)
            {
                dict.ToList().ForEach(pair => result[pair.Key] = pair.Value);
            }

            return result;
        }

        /*
        Seteo dinamico

        https://stackoverflow.com/questions/1825952/how-to-create-a-generic-extension-method
        */
        public static void SetValue<T>(this T sender, string propertyName, object value)
        {
            var propertyInfo = sender!.GetType().GetProperty(propertyName);

            if (propertyInfo is null) return;

            var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(sender, Enum.Parse(propertyInfo.PropertyType, value.ToString()!));
            }
            else
            {
                var safeValue = (value == null) ? null : Convert.ChangeType(value, type);
                propertyInfo.SetValue(sender, safeValue, null);
            }
        }

        public static void SetConfig<T>(this T sender, Dictionary<string, object> config)
        {
            foreach (KeyValuePair<string, object> c in config)
            {
                if (c.Key.Contains("+"))
                {
                    string k = c.Key.TrimEnd(new Char[] { '+' });
                    if (!config.ContainsKey(k)) config[k] = new List<object>();
                    List<object> conf = JsonConvert.DeserializeObject<List<object>>(config[k].ToString());
                    foreach (object v in JsonConvert.DeserializeObject<List<object>>(c.Value.ToString()))
                    {
                        if (!conf.Contains(v))
                        {
                            conf.Add(v);
                        }
                    }
                    config[k] = conf;
                }
                else
                {
                    if (c.Key.Contains("-"))
                    {
                        string k = c.Key.TrimEnd(new Char[] { '-' });
                        if (!config.ContainsKey(k)) config[k] = new List<object>();
                        List<object> conf = (List<object>)config[k];
                        config[k] = conf.Except((List<object>)c.Value).ToList();
                    }
                }

                sender.SetValue(c.Key, c.Value);
            }
        }
    }
}
