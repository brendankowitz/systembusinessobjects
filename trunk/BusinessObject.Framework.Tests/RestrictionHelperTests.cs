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

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .IsNotNull(p => p.FirstName)
                .Criteria;

            c.UniqueResult();
        }

        [Test]
        public void CanUseEqlambdaExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .Eq(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpression()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Eq(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpression2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Eq(p => p.FirstName == "John")
                .IsNotNull(p => p.LastName)
                .Add(p => p.ID > 0 && p.ID < 1000)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID > 0 && p.ID < 1000)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID < 1000 && p.ID > 0)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        [ExpectedException(typeof(NotSupportedException))]
        public void CanUseEqlambdaExtensionExpressionWhere3()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID < 1000 && p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere4()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID != null)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere5()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID != 0)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere6()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            string john = "John";

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == john)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere7()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            string john = "Jo%";

            ICriteria c = session.CreateExpression<Person>()
                .Like(p => p.FirstName == john)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpressionWhere8()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.AutoFlush = false;
            pers.Save();

            string john = "ab%";

            ICriteria c = session.CreateExpression<Person>()
                .Like(p => p.FirstName != john)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }
#endif
    }
}
