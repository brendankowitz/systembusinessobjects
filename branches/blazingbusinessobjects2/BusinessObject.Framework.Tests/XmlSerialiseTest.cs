using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Sample.BusinessObjects.Contacts;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class XmlSerialiseTest : NHibernateInMemoryTestFixtureBase
    {
        [Fact]
        public void Serialise()
        {
            StringWriter writer = new StringWriter();
            Person obj = BusinessObjectFactory.CreateAndFillPerson();
            obj.ID = 12;

            new XmlSerializer(typeof(Person)).Serialize(writer, obj);

            writer.Close();

            Trace.WriteLine(writer.ToString());
        }

        [Fact]
        public void SerialiseFromObject()
        {
            Person obj = BusinessObjectFactory.CreateAndFillPerson();
            obj.ID = 12;

            string xml = obj.SerializeToXml();

            Person p2 = Person.DeserializeFromXml(xml);

            BusinessObjectFactory.CheckPerson(obj, p2);
        }
    }
}
