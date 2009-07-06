using System;
using System.IO;
using System.Xml.Serialization;

namespace System.BusinessObjects.Helpers
{
    public class XmlHelper
    {
        private static XmlSerializer CreateXMLSerializer(Type typeToSerialise, string pStrXMLRootName)
        {
            XmlSerializer serializer;
            if (pStrXMLRootName != null && pStrXMLRootName != string.Empty)
            {
                serializer = new XmlSerializer(typeToSerialise,pStrXMLRootName);
            }
            else
            {
                serializer = new XmlSerializer(typeToSerialise);
            }
            return serializer;
        }

        private static XmlRootAttribute CreateXMLRootAttribute(string psRootName)
        {
            // Create an XmlRootAttribute overloaded constructer 
            //and set its namespace.
            XmlRootAttribute newXmlRootAttribute = null;
            if (psRootName != null && psRootName != string.Empty)
            {
                newXmlRootAttribute = new XmlRootAttribute(psRootName);
            }
            return newXmlRootAttribute;
        }

        public static string SerializeToXML(object obj)
        {
            StringWriter writer = new StringWriter();
            CreateXMLSerializer(obj.GetType(), null).Serialize(writer, obj);
            writer.Close();
            return writer.ToString();
        }

        public static string SerializeToXML(object obj, string pStrXMLRootName)
        {
            StringWriter writer = new StringWriter();
            CreateXMLSerializer(obj.GetType(), pStrXMLRootName).Serialize(writer, obj);
            writer.Close();
            return writer.ToString();
        }

        public static T Deserialise<T>(string xml)
        {
            StringReader rd = new StringReader(xml);
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), null);
            deserialisedObj = (T)serialiser.Deserialize(rd);
            return deserialisedObj;
        }

        public static T Deserialise<T>(Stream stream)
        {
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), null);
            deserialisedObj = (T)serialiser.Deserialize(stream);
            return deserialisedObj;
        }

        public static T Deserialise<T>(string pStrXMLRootName, Stream stream)
        {
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), pStrXMLRootName);
            deserialisedObj = (T)serialiser.Deserialize(stream);
            return deserialisedObj;
        }

        public static T Deserialise<T>(string pStrXMLRootName, string xml)
        {
            StringReader rd = new StringReader(xml);
            T deserialisedObj;
            XmlSerializer serialiser = CreateXMLSerializer(typeof(T), pStrXMLRootName);
            deserialisedObj = (T)serialiser.Deserialize(rd);
            return deserialisedObj;
        }

    }
}
