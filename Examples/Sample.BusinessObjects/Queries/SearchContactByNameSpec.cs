using NHibernate;
using Sample.BusinessObjects.Contacts;
using NHibernate.Criterion;
using System.BusinessObjects.Data;
using System.Collections.Generic;
using System.BusinessObjects.Infrastructure;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace Sample.BusinessObjects.Queries
{
    /// <summary>
    /// Provides a query to search for contacts by name
    /// </summary>
    public class SearchContactByNameSpec : Specification<Person>
    {
        private readonly string _name;
        public SearchContactByNameSpec(string name)
        {
            _name = name;
        }

        //IEnumerable<Person> IDataQuery<Person>.Execute()
        //{
        //    ICriteria qry = ((IRequiresNHibernateSession)this).Session.CreateCriteria(typeof(Person));

        //    if (!string.IsNullOrEmpty(_name))
        //    {
        //        string nameWildcard = string.Format("%{0}%", _name);
        //        qry.Add(Expression.Or(Expression.Like("FirstName", nameWildcard), Expression.Like("LastName", nameWildcard)));
        //    }
        //    qry.AddOrder(Order.Asc("FirstName"));

        //    return qry.List<Person>();
        //}

        public override Expression<Func<Person, bool>> Predicate
        {
            get 
            { 
                string nameWildcard = string.Format("%{0}%", _name);
                return x => x.FirstName.Contains(nameWildcard);
            }
        }
    }
}
