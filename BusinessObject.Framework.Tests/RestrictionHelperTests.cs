using System;
using Sample.BusinessObjects.Contacts;
using NHibernate;
using NHibernate.Criterion;
using System.BusinessObjects.Expressions;
using NHibernate.Impl;
using Xunit;
using System.BusinessObjects.Data;
using Xunit.Extensions;

namespace BusinessObject.Framework.Tests
{
    public class RestrictionHelperTests : NHibernateInMemoryTestFixtureBase
    {
        public string QueryFirstNameProperty{ get{ return "John"; } }
        IDataObjectRepository<Person> _repository;

        public RestrictionHelperTests()
        {
            _repository = new NHExpressionsRepository<Person>(session);
        }

#if DOT_NET_35
        [Fact, AutoRollback]
        public void CanUseEqStrongProperty()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);

            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.GetFor(() => new Person().FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);

        }

        [Fact, AutoRollback]
        public void CanUseEqStrongProperty2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(Restrictions.Eq(System.BusinessObjects.Helpers.Property.For<Person>(p => p.FirstName), "John"));

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);

        }

        [Fact, AutoRollback]
        public void CanUseEqLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.FirstName == "John" ));
            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
            
        }

        [Fact, AutoRollback]
        public void CanUseEqLambda2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => "John" == p.FirstName));
            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);

        }

        [Fact, AutoRollback]
        public void CanUseGeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID >= 0));

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);

        }

        [Fact, AutoRollback]
        public void CanUseGtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID > 0));

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);

        }

        [Fact]
        public void CanUseLeLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID <= 0));

            Person result = c.UniqueResult<Person>();
            Assert.Null(result);

        }

        [Fact]
        public void CanUseLtLambda()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person));
            c.Add(RestrictBy.Add<Person>(p => p.ID < 0));

            Person result = c.UniqueResult<Person>();
            Assert.Null(result);

        }

        //[Fact]
        //public void CanUseIsNulllambda()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();

        //    ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
        //    c.Add(RestrictBy.Add<Person>(p => p.FirstName == null));

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Null(result);
        //}

        //[Fact]
        //public void CanUseIsNotNull()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));
        //    c.Add(RestrictBy.Add<Person>(p => p.FirstName != null));

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Equal("John", result.FirstName);
        //}

        [Fact]
        public void CanUseIsNotNullExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);

            ICriteria c = session.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .Add(p => p.FirstName != null)
                .Criteria;

            c.UniqueResult();
        }

        [Fact]
        public void CanUseEqlambdaExtension()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateCriteria(typeof(Person))
                .Expression<Person>()
                .Add(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpression()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == "John")
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpression_Multiple_Adds()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == "John")
                .Add(p => p.LastName != null)
                .Add(p => p.ID > 0 && p.ID < 1000)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID > 0 && p.ID < 1000)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID < 1000 && p.ID > 0)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere3()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();

            Assert.Throws<NotSupportedException>(() =>
            {
                ICriteria c = session.CreateExpression<Person>()
                    .Add(p => p.ID < 1000 && p.FirstName == "John")
                    .Criteria;
            });
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere4()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID != null)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere5()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.ID != 0)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere6()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            string john = "John";

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName == john)
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere7()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            string john = "Jo%";

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName.Contains(john))
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        [Fact]
        public void CanUseEqlambdaExtensionExpressionWhere8()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);
            string john = "ab%";

            ICriteria c = session.CreateExpression<Person>()
                .Add(p => !p.FirstName.Contains(john))
                .Criteria;

            Person result = c.UniqueResult<Person>();
            Assert.Equal("John", result.FirstName);
        }

        //[Fact]
        //public void CanUseIsNotNull2()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    ICriteria c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
        //        .Expression<Person>(p => p.FirstName != null);

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Equal("John", result.FirstName);
        //}

        //[Fact]
        //public void CanUseIsNotNull_WithOrder()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
        //        .Add(p => p.FirstName != null)
        //        .OrderDesc(p => p.LastName)
        //        .Criteria;

        //    Assert.Equal("FirstName is not null\r\nLastName desc", c.ToString());

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Equal("John", result.FirstName);
        //}

        [Fact]
        public void CanUseAlias()
        {
            //Using expression interface
            ICriteria c = session.CreateExpression<Person>()
                .Add(p => p.FirstName = "John")
                .Alias<Address>(p => p.Addresses)
                    .Add(a => a.Postcode != null)
                    .AddAndReturn(a => a.Address1 == null)
                .OrderAsc(p => p.ID)
                .Criteria;

            //Normal interface
            ICriteria o = session.CreateCriteria(typeof(Person))
                .Add(Restrictions.Eq("FirstName", "John"))
                .CreateAlias("Addresses", "Addresses0")
                    .Add(Restrictions.IsNotNull("Addresses0.Postcode"))
                    .Add(Restrictions.IsNull("Addresses0.Address1"))
                .AddOrder(Order.Asc("ID"));

            Console.WriteLine(c.ToString());
            Assert.Equal(c.ToString(), o.ToString());
        }

        //[Fact]
        //public void CanUseProjection()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    //Using expression interface
        //    ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
        //        .SetProjection(Projections.Min, p => p.ID)
        //        .Criteria;

        //    ICriteria o = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
        //        .SetProjection(Projections.Min("ID"));

        //    Assert.Equal(((CriteriaImpl)o).Projection.ToString(), ((CriteriaImpl)c).Projection.ToString());
        //}

        //[Fact]
        //public void CanUseParameter()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    ICriteria c = null;
        //    Action<string> del = (name) =>
        //    {
        //        c = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
        //            .Expression<Person>(p => p.FirstName == name);
        //    };

        //    del("John");

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Equal("John", result.FirstName);
        //}

        //[Fact]
        //public void CanUseProperty()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    ICriteria c = UnitOfWork.CurrentSession.CreateExpression<Person>()
        //        .Add(p => p.FirstName == QueryFirstNameProperty)
        //        .Criteria;

        //    Person result = c.UniqueResult<Person>();
        //    Assert.Equal("John", result.FirstName);
        //}

        //[Fact]
        //public void DetachedCriteria()
        //{
        //    Person pers = BusinessObjectFactory.CreateAndFillPerson();
        //    pers.SetSession(session);
        //    pers.Save();

        //    DetachedCriteria d = DetachedCriteriaExpression<Person>.Create()
        //        .Add(p => p.FirstName == "John")
        //        .Alias<Address>(p => p.Addresses, "addr")
        //            .Add(a => a.Postcode != null)
        //            .AddAndReturn(a => a.Address1 == null)
        //        .OrderAsc(p => p.ID)
        //        .Criteria;

        //    //convert to criteria
        //    ICriteria c = d.GetExecutableCriteria(session);

        //    //Normal interface
        //    ICriteria o = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person))
        //        .Add(Restrictions.Eq("FirstName", "John"))
        //        .CreateAlias("Addresses", "addr")
        //            .Add(Restrictions.IsNotNull("addr.Postcode"))
        //            .Add(Restrictions.IsNull("addr.Address1"))
        //        .AddOrder(Order.Asc("ID"));

        //    Console.WriteLine(c.ToString());
        //    Assert.Equal(c.ToString(), o.ToString());
        //}

        [Fact]
        public void UsingSpecification()
        {
            Sample.BusinessObjects.Queries.ContactByIdSpec spec = new Sample.BusinessObjects.Queries.ContactByIdSpec(1);

            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);

            var result = _repository.Fetch(spec);
            
            Assert.Equal("John", result.FirstName);

        }
#endif
    }
}
