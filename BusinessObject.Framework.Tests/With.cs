using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.BusinessObjects.With;
using Sample.BusinessObjects.Contacts;

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
        public void TestEachFunction()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            With.Each(list).Item((EachItemDelegate<Person>)ChangePersonName);

            Assert.AreEqual("Changed", list[0].FirstName);
        }

        public void ChangePersonName(Person person)
        {
            person.FirstName = "Changed";
        }
    }
}
