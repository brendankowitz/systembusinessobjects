using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;
using NHibernate;
using System.BusinessObjects.Transactions;
using NHibernate.Criterion;
using System.BusinessObjects.Expressions;
using NHibernate.Impl;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class RestrictionHelperTests : NHibernateInMemoryTestFixtureBase
    {
        public string QueryFirstNameProperty{ get{ return "John"; } }

#if DOT_NET_35
        [Test]
        public void CanUseEqStrongProperty()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
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
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.For<Person>(p => p.FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseEqLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.FirstName == "John" ));
            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
            
        }

        [Test]
        public void CanUseEqLambda2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => "John" == p.FirstName));
            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseGeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID >= 0));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseGtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID > 0));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);

        }

        [Test]
        public void CanUseLeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID <= 0));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);

        }

        [Test]
        public void CanUseLtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID < 0));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);

        }

        [Test]
        public void CanUseIsNulllambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.FirstName == null));

            Person result = c.UniqueResult<Person>();
            Assert.IsNull(result);
        }

        [Test]
        public void CanUseIsNotNull()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.FirstName != null));

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseIsNotNullExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .Add(p => p.FirstName != null)
                .Criteria;

            c.UniqueResult();
        }

        [Test]
        public void CanUseEqlambdaExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .Add(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpression()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseEqlambdaExtensionExpression_Multiple_Adds()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == "John")
                .Add(p => p.LastName != null)
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
            pers.Save();

            string john = "ab%";

            ICriteria c = session.CreateExpression<Person>()
                .Like(p => p.FirstName != john)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseIsNotNull2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .Expression<Person>(p => p.FirstName != null);

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseIsNotNull_WithOrder()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
                .Add(p => p.FirstName != null)
                .OrderDesc(p => p.LastName)
                .Criteria;

            Assert.AreEqual("FirstName is not null\r\nLastName desc", c.ToString());

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseAlias()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            //Using expression interface
            ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
                .Add(p => p.FirstName == "John")
                .Alias<Address>(p => p.Addresses, "addr")
                    .Add(a => a.Postcode != null)
                    .AddAndReturn(a => a.Address1 == null)
                .OrderAsc(p => p.ID)
                .Criteria;

            //Normal interface
            ICriteria o = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .Add(Restrictions.Eq("FirstName", "John"))
                .CreateAlias("Addresses", "addr")
                    .Add(Restrictions.IsNotNull("addr.Postcode"))
                    .Add(Restrictions.IsNull("addr.Address1"))
                .AddOrder(Order.Asc("ID"));

            Console.WriteLine(c.ToString());
            Assert.AreEqual(c.ToString(), o.ToString());
        }

        [Test]
        public void CanUseProjection()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            //Using expression interface
            ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
                .SetProjection(Projections.Min, p => p.ID)
                .Criteria;

            ICriteria o = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .SetProjection(Projections.Min("ID"));

            Assert.AreEqual(((CriteriaImpl)o).Projection.ToString(), ((CriteriaImpl)c).Projection.ToString());
        }

        [Test]
        public void CanUseParameter()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = null;
            Action<string> del = (name) =>
            {
                c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                    .Expression<Person>(p => p.FirstName == name);
            };

            del("John");

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void CanUseProperty()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
                .Add(p => p.FirstName == QueryFirstNameProperty)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.AreEqual("John", result.FirstName);
        }

        [Test]
        public void DetachedCriteria()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            pers.SetSession(session);
            pers.Save();

            DetachedCriteria d = DetachedCriteriaExpression<Person>.Create()
                .Add(p => p.FirstName == "John")
                .Alias<Address>(p => p.Addresses, "addr")
                    .Add(a => a.Postcode != null)
                    .AddAndReturn(a => a.Address1 == null)
                .OrderAsc(p => p.ID)
                .Criteria;

            //convert to criteria
            ICriteria c = d.GetExecutableCriteria(session);

            //Normal interface
            ICriteria o = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
                .Add(Restrictions.Eq("FirstName", "John"))
                .CreateAlias("Addresses", "addr")
                    .Add(Restrictions.IsNotNull("addr.Postcode"))
                    .Add(Restrictions.IsNull("addr.Address1"))
                .AddOrder(Order.Asc("ID"));

            Console.WriteLine(c.ToString());
            Assert.AreEqual(c.ToString(), o.ToString());
        }
#endif
    }
}
