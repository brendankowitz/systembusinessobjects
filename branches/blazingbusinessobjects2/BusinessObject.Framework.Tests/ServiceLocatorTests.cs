using System;
using System.Collections.Generic;
using System.Text;
using System.BusinessObjects.Providers;
using System.BusinessObjects.Data;

namespace BusinessObject.Framework.Tests
{
    public class ServiceLocatorTests : NHibernateInMemoryTestFixtureBase
    {
        //[Fact]
        //public void CanLocateDataCache()
        //{
        //    Assert.IsNotNull(ServiceLocator.CacheProvider);
        //    Assert.IsAssignableFrom(typeof(HashtableDataCache), ServiceLocator.CacheProvider);
        //    Assert.Equal(true, ServiceLocator.CacheProvider.UseCache);
        //}

        //[Fact]
        //public void CanLocateSessionProvider()
        //{
        //    Assert.IsNotNull(ServiceLocator.NHibernateSessionProvider);
        //    Assert.IsAssignableFrom(typeof(NHibernateThreadSlotProvider), ServiceLocator.NHibernateSessionProvider);
        //}

    }
}