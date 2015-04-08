using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace ForestInhabitant
{
    public static class Serializer
    {
        public static byte[] Serialize<T>(T obj)
        {
            var ms = new MemoryStream();
            using (var writer = new BsonWriter(ms))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, obj);
            }
            return ms.ToArray();
        }

        public static T Deserialize<T>(byte[] data)
        {
            var ms = new MemoryStream(data);
            using (var reader = new BsonReader(ms))
            {
                var serializer = new JsonSerializer();
                T a;
                try
                {
                    a = serializer.Deserialize<T>(reader);
                }
                catch (Exception)
                {
                    Console.WriteLine("EXCEPTION");
                    Console.WriteLine(data.TakeWhile(x => x != 0).Count());
                    return serializer.Deserialize<T>(reader);
                }
                return a;
            }
        }
    }
}
