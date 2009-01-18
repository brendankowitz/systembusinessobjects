using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.BusinessObjects.Transactions;
using Sample.BusinessObjects.Contacts;
using System.BusinessObjects.With;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class UnitOfWorkTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
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
