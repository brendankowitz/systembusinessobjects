using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.BusinessObjects.Providers;
using System.BusinessObjects.Data;

namespace BusinessObject.Framework.Tests
{
    public class ServiceLocatorTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void CanLocateDataCache()
        {
            Assert.IsNotNull(ServiceLocator.CacheProvider);
            Assert.IsAssignableFrom(typeof(HashtableDataCache), ServiceLocator.CacheProvider);
            Assert.AreEqual(true, ServiceLocator.CacheProvider.UseCache);
        }

        [Test]
        public void CanLocateSessionProvider()
        {
            Assert.IsNotNull(ServiceLocator.NHibernateSessionProvider);
            Assert.IsAssignableFrom(typeof(NHibernateThreadSlotProvider), ServiceLocator.NHibernateSessionProvider);
        }

    }
}
