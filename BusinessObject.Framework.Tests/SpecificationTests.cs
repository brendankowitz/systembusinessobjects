using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.BusinessObjects.Data;
using Sample.BusinessObjects.Contacts;
using Xunit;
using Sample.BusinessObjects.Queries;

namespace BusinessObject.Framework.Tests
{
    public class SpecificationTests : NHibernateInMemoryTestFixtureBase
    {
        public string QueryFirstNameProperty{ get{ return "John"; } }
        IDataObjectRepository<Person> _repository;

        public SpecificationTests()
        {
            _repository = new NHExpressionsRepository<Person>(session);
        }


        [Fact]
        public void CanUseEqStrongProperty2()
        {
            Person pers = BusinessObjectFactory.CreateAndFillPerson();
            _repository.Save(pers);

            var spec = new SearchContactByNameSpec("John");

            var result = _repository.Fetch(spec);
            Assert.Equal("John", result.FirstName);

        }
    }
}
