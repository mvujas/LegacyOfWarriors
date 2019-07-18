using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;

namespace Utils
{
    public static class SeriabilityUtils
    {
        public static byte[] ObjectToByteArray<T>(T obj)
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static T ByteArrayToObject<T>(byte[] arrBytes)
        {
            return JsonConvert.DeserializeObject<T>(Encoding.ASCII.GetString(arrBytes));
        }
    }
}
