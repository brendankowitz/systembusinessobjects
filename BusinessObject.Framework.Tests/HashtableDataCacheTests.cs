using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.BusinessObjects.Data;
using Sample.BusinessObjects.Contacts;
using System.Threading;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class HashtableDataCacheTests : NHibernateInMemoryTestFixtureBase
    {
        HashtableDataCache cache;

        [Test]
        public void CacheStringFromArray()
        {
            List<int> items = new List<int>();
            items.AddRange(new int[]{ 1, 2, 3 });

            string retval = HashtableDataCache.CreateCacheString("test", items.ToArray());

            Assert.AreEqual("test_1_2_3", retval);
        }

        [Test]
        public void AddItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);

            Assert.AreEqual(1, cache.ItemCount);
        }

        [Test]
        public void RemoveItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);

            Assert.AreEqual(1, cache.ItemCount);

            cache.RemoveCache("person");

            Assert.AreEqual(0, cache.ItemCount);
        }

        [Test]
        public void ExpiresItem_BusinessObject()
        {
            cache = new HashtableDataCache();
            cache.DefaultTimeout = 1;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);
            Assert.AreEqual(1, cache.ItemCount);

            Thread.Sleep(1001);
            Assert.IsNull(cache.GetCache<Person>("person"));
            Assert.AreEqual(0, cache.ItemCount);
        }

        [Test]
        public void RemovesOldItems()
        {
            cache = new HashtableDataCache();
            cache.DefaultTimeout = 1;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            cache.SetCache("person", p);
            Assert.AreEqual(1, cache.ItemCount);

            Thread.Sleep(1001);

            cache.RemoveOldItems();
            Assert.AreEqual(0, cache.ItemCount);
        }
    }
}
