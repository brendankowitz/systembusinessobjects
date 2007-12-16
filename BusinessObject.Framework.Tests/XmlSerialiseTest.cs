using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class XmlSerialiseTest : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void Serialise()
        {
            StringWriter writer = new StringWriter();
            Person obj = BusinessObjectFactory.CreateAndFillPerson();
            obj.ID = 12;

            new XmlSerializer(typeof(Person)).Serialize(writer, obj);

            writer.Close();

            Trace.WriteLine(writer.ToString());
        }

        [Test]
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
