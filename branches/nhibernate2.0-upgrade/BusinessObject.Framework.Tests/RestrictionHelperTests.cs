using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;
using NHibernate;
using System.BusinessObjects.Transactions;
using System.BusinessObjects.Helpers;
using NHibernate.Criterion;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class RestrictionHelperTests : NHibernateInMemoryTestFixtureBase
    {
#if DOT_NET_35
        [Test]
        public void CanUseEqStrongProperty()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.GetFor(() => new Person().FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseEqlambda()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq(() => new Person().FirstName == "John" ));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
            
        }

        [Test]
        public void CanUseIsNulllambda()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.IsNull(() => new Person().FirstName));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);
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

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

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
        public void CanUseEqlambdaExtension()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.SetSession(session);
            p.AutoFlush = false;
            p.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.AddEq(() => new Person().FirstName == "John");

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }
#endif
    }
}
