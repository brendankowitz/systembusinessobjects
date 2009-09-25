using System;
using System.BusinessObjects.Data;
using Sample.BusinessObjects.Contacts;
using System.Threading;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class HashtableDataCacheTests : NHibernateInMemoryTestFixtureBase
    {
        HashtableDataCache cache;


        [Fact]
        public void AddItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);

            Assert.Equal(1, cache.ItemCount);
        }

        [Fact]
        public void RemoveItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);

            Assert.Equal(1, cache.ItemCount);

            cache.RemoveCache("person");

            Assert.Equal(0, cache.ItemCount);
        }

        [Fact]
        public void ExpiresItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            cache.DefaultTimeout = 1;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);
            Assert.Equal(1, cache.ItemCount);

            Thread.Sleep(1100);
            Assert.Null(cache.GetCache<Person>("person"));
            Assert.Equal(0, cache.ItemCount);
        }

        [Fact]
        public void RemovesOldItems()
        {
            cache = new HashtableDataCache();
            cache.DefaultTimeout = 1;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);
            Assert.Equal(1, cache.ItemCount);

            Thread.Sleep(1100);

            cache.RemoveOldItems();
            Assert.Equal(0, cache.ItemCount);
        }
    }
}
