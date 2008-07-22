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

        [Test]
        public void TestWithSplit()
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);

            IList<int>[] array = With.Each(list).Split<int>(3);

            Assert.AreEqual(3, array[0].Count);
            Assert.AreEqual(2, array[1].Count);

            Assert.AreEqual(1, array[0][0]);
            Assert.AreEqual(4, array[1][0]);
        }

        public void ChangePersonName(Person person)
        {
            person.FirstName = "Changed";
        }

        public string FormatPerson(Person person)
        {
            return person.FirstName + " " + person.LastName;
        }

#if DOT_NET_35
        [Test]
        public void TestEachFunctionCollectionExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = list.Each().Item<Person, string>(FormatPerson) as IList<string>;

            Assert.AreEqual("John Smith", outputlist[0]);
        }

        [Test]
        public void TestlambdaEachFunctionCollectionExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            list.Each<Person>(x => x.FirstName = "Change");

            Assert.AreEqual("Change", list[0].FirstName);
        }

        [Test]
        public void TestlambdaEachFunctionCollectionConvertExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            list.Each<Person>(x => x.FirstName = "Change");

            Assert.AreEqual("Change", list[0].FirstName);

        }

        [Test]
        public void TestlambdaEachFunctionCollectionConvertOutputExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = list.Each().Item<Person, string>(p => p.FirstName + "." + p.LastName) as IList<string>;
            Assert.AreEqual("John.Smith", outputlist[0]);
        }
#endif

    }
}
