using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;
using NHibernate;
using System.BusinessObjects.Transactions;
using System.BusinessObjects.Helpers;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class RestrictionHelperTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void CanUseEqLamba()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq(() => new Person().FirstName == "John" ));

            c.UniqueResult();
        }

        [Test]
        public void CanUseIsNullLamba()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.IsNull(() => new Person().FirstName));

            c.List();
        }

        [Test]
        public void CanUseIsNotNull()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.IsNotNull(() => new Person().FirstName));

            c.UniqueResult();
        }

#if DOT_NET_35
        [Test]
        public void CanUseIsNotNullExtension()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.AddIsNotNull(() => new Person().FirstName);

            c.UniqueResult();
        }
        public void CanUseEqLambaExtension()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.AddEq(() => new Person().FirstName == "John");

            c.UniqueResult();
        }
#endif
    }
}
