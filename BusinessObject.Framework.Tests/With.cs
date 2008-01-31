using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.BusinessObjects.With;
using Sample.BusinessObjects.Contacts;
using System.Diagnostics;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class WithTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void TestEachDelegate()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            With.Each(list).Item(delegate(Person person)
            {
                person.FirstName = "Changed";
            });

            Assert.AreEqual("Changed", list[0].FirstName);
        }

        [Test]
        public void TestEachHandleNullArray()
        {
            With.Each(null).Item(delegate(Person person)
            {
                person.FirstName = "Changed";
            });
        }

        [Test]
        public void TestEachFunction()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            With.Each(list).Item<Person>(ChangePersonName);

            Assert.AreEqual("Changed", list[0].FirstName);
        }

        [Test]
        public void TestEachFunctionCollection()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = With.Each(list).Item<Person, string>(FormatPerson) as IList<string>;

            Assert.AreEqual("John Smith", outputlist[0]);
        }

        [Test]
        public void TestEachFunctionCollection_WithExistingList()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> myConvertToList = new List<string>();

            IList<string> outputlist = With.Each(list).Item<Person, string>(myConvertToList, FormatPerson) as IList<string>;

            Assert.AreEqual("John Smith", myConvertToList[0]);
        }

        public void ChangePersonName(Person person)
        {
            person.FirstName = "Changed";
        }

        public string FormatPerson(Person person)
        {
            return person.FirstName + " " + person.LastName;
        }
    }
}
