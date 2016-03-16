using System;
using System.IO;
using System.Xml.Serialization;

namespace Engine.Manager
{
    public class XmlManager<T>
    {
        public Type Type;

        public XmlManager()
        {
            Type = typeof(T);
        }

        public T Load(string path)
        {
            T instance;
            try
            {
                using (TextReader reader = new StreamReader(path))
                {
                    var xml = new XmlSerializer(Type);
                    instance = (T) xml.Deserialize(reader);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            return instance;
        }

        public void Save(string path, object obj)
        {
            using (TextWriter writer = new StreamWriter(path))
            {
                var xml = new XmlSerializer(Type);
                xml.Serialize(writer, obj);
            }
        }
    }
}