using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Motion.Core
{
    public class SerializerJson
    {
        public static bool SaveToJson(object o, string path)
        {
            try
            {

                File.WriteAllText(path, JsonConvert.SerializeObject(o));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static T LoadFromJson<T>(string path)
        {
            if (!File.Exists(path))
            {
                return default(T);
            }
            try
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

    }
}
