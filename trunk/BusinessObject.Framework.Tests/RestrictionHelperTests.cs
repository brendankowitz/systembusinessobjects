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
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.GetFor(() => new Person().FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseEqStrongProperty2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.GetFor<Person>(p => p.FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseEqLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => p.FirstName == "John" ));
            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
            
        }

        [Test]
        public void CanUseEqLambda2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => "John" == p.FirstName));
            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseGeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => p.ID >= 0));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseGtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => p.ID > 0));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseLeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => p.ID <= 0));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);

        }

        [Test]
        public void CanUseLtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Eq<Person>(p => p.ID < 0));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);

        }

        [Test]
        public void CanUseIsNulllambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.IsNull<Person>(p => p.FirstName));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);
        }

        [Test]
        public void CanUseIsNotNull()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.IsNotNull<Person>(p => p.FirstName));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseIsNotNullExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.AddIsNotNull<Person>(p => p.FirstName);

            c.UniqueResult();
        }
        public void CanUseEqlambdaExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.AddEq<Person>(p => p.FirstName == "John");

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }
#endif
    }
}
