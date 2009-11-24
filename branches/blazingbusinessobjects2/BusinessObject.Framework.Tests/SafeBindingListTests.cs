using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sample.BusinessObjects.Contacts;
using System.BusinessObjects.Data;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class SafeBindingListTests : NHibernateInMemoryTestFixtureBase
    {
        [Fact]
        public void CreateSafeList_UsingProxies()
        {
            Castle.DynamicProxy.ProxyGenerator generator = new Castle.DynamicProxy.ProxyGenerator();

            Person obj1 = BusinessObjectFactory.CreateAndFillPerson();
            Person obj2 = BusinessObjectFactory.CreateAndFillPerson();
            Person proxObj = generator.CreateClassProxy<Person>(new DataObjectInterceptor(obj2));

            IList<Person> safeList = SafeDataBindingList<Person>.Create(new List<Person>() { obj1, proxObj });

            //Should have converted both to proxies
            Assert.NotEqual(obj1.GetType(), safeList[0].GetType());
            Assert.True(safeList[0].GetType() == safeList[1].GetType());
        }

        [Fact]
        public void CreateSafeList_DoNothing()
        {
            Castle.DynamicProxy.ProxyGenerator generator = new Castle.DynamicProxy.ProxyGenerator();

            Person obj1 = BusinessObjectFactory.CreateAndFillPerson();
            Person obj2 = BusinessObjectFactory.CreateAndFillPerson();

            IList<Person> safeList = SafeDataBindingList<Person>.Create(new List<Person>() { obj1, obj2 });

            //Should have converted both to proxies
            Assert.Equal(obj1.GetType(), safeList[0].GetType());
            Assert.Equal(obj2.GetType(), safeList[1].GetType());

            Assert.True(safeList[0].GetType() == safeList[1].GetType());
        }
    }
}
