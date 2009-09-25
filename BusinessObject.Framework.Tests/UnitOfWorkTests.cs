using System;
using System.BusinessObjects.Transactions;
using Sample.BusinessObjects.Contacts;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class UnitOfWorkTests : NHibernateInMemoryTestFixtureBase
    {
        [Fact]
        public void SimpleDelegate()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();

            Transaction.Execute(() =>
            {
                p.Save();
            });

            Person p2 = Person.Load(p.ID);
            BusinessObjectFactory.CheckPerson(p, p2);
        }
    }
}
