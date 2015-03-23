using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Shared
{
    public class Serializer
    {
        public static byte[] Serialize<T>(T serializeMe)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof (T));
                serializer.Serialize(stream, serializeMe);
                stream.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                return stream.GetBuffer();
            }
        }

        public static T Deserialize<T>(byte[] deSerializeMe)
        {
            using (var stream = new MemoryStream(deSerializeMe))
            {
               
                stream.Seek(0, SeekOrigin.Begin);

                var reader = new StreamReader(stream);
                Debug.WriteLine("Message: " + reader.ReadToEnd());

                stream.Seek(0, SeekOrigin.Begin);

                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(stream);
            }
        }
    }
}
