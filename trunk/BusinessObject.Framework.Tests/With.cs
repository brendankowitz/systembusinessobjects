using System;
using System.Collections.Generic;
using System.BusinessObjects.With;
using Sample.BusinessObjects.Contacts;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class WithTests : NHibernateInMemoryTestFixtureBase
    {
        [Fact]
        public void TestEachDelegate()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            With.Each(list).Item(delegate(Person person)
            {
                person.FirstName = "Changed";
            });

            Assert.Equal("Changed", list[0].FirstName);
        }

        [Fact]
        public void TestEachHandleNullArray()
        {
            With.Each(null).Item(delegate(Person person)
            {
                person.FirstName = "Changed";
            });
        }

        [Fact]
        public void TestEachFunction()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            With.Each(list).Item<Person>(ChangePersonName);

            Assert.Equal("Changed", list[0].FirstName);
        }

        [Fact]
        public void TestEachFunctionCollection()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = With.Each(list).Item<Person, string>(FormatPerson) as IList<string>;

            Assert.Equal("John Smith", outputlist[0]);
        }

        [Fact]
        public void TestEachFunctionCollection_WithExistingList()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> myConvertToList = new List<string>();

            IList<string> outputlist = With.Each(list).Item<Person, string>(myConvertToList, FormatPerson) as IList<string>;

            Assert.Equal("John Smith", myConvertToList[0]);
        }

        [Fact]
        public void TestWithSplit()
        {
            List<int> list = new List<int>();
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Add(4);
            list.Add(5);

            IList<int>[] array = With.Each(list).Split<int>(3);

            Assert.Equal(3, array[0].Count);
            Assert.Equal(2, array[1].Count);

            Assert.Equal(1, array[0][0]);
            Assert.Equal(4, array[1][0]);
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
        [Fact]
        public void TestEachFunctionCollectionExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = list.Each().Item<Person, string>(FormatPerson) as IList<string>;

            Assert.Equal("John Smith", outputlist[0]);
        }

        [Fact]
        public void TestlambdaEachFunctionCollectionExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            list.Each<Person>(x => x.FirstName = "Change");

            Assert.Equal("Change", list[0].FirstName);
        }

        [Fact]
        public void TestlambdaEachFunctionCollectionConvertExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            list.Each<Person>(x => x.FirstName = "Change");

            Assert.Equal("Change", list[0].FirstName);

        }

        [Fact]
        public void TestlambdaEachFunctionCollectionConvertOutputExtension()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            IList<string> outputlist = list.Each().Item<Person, string>(p => p.FirstName + "." + p.LastName) as IList<string>;
            Assert.Equal("John.Smith", outputlist[0]);
        }

        [Fact]
        public void TestlambdaEachFunctionCollectionPredicateSelect()
        {
            List<Person> list = new List<Person>();
            list.Add(BusinessObjectFactory.CreateAndFillPerson());

            Person selected = list.FirstMatch(x => x.FirstName == "John");

            Assert.Equal("John", selected.FirstName);

        }
#endif

    }
}
