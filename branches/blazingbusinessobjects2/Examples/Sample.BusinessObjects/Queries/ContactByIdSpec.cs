using System;
using System.Linq;
using Sample.BusinessObjects.Contacts;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;

namespace Sample.BusinessObjects.Queries
{
    public class ContactByIdSpec : Specification<Person>
    {
        private int _contactId;
        public ContactByIdSpec(int contactId)
        {
            _contactId = contactId;
        }

        public override Expression<Func<Person, bool>> Predicate
        {
            get { return x => x.ID == _contactId; }
        }
    }
}
