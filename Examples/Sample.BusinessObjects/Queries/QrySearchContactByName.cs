using NHibernate;
using Sample.BusinessObjects.Contacts;
using NHibernate.Criterion;
using System.BusinessObjects.Data;
using System.Collections.Generic;

namespace Sample.BusinessObjects.Queries
{
    /// <summary>
    /// Provides a query to search for contacts by name
    /// </summary>
    public class QrySearchContactByName : IDataQuery<Person>, IRequiresNHibernateSession
    {
        private readonly string _name;
        ISession IRequiresNHibernateSession.Session { get; set; }

        public QrySearchContactByName(string name)
        {
            _name = name;
        }

        IEnumerable<Person> IDataQuery<Person>.Execute()
        {
            ICriteria qry = ((IRequiresNHibernateSession)this).Session.CreateCriteria(typeof(Person));

            if (!string.IsNullOrEmpty(_name))
            {
                string nameWildcard = string.Format("%{0}%", _name);
                qry.Add(Expression.Or(Expression.Like("FirstName", nameWildcard), Expression.Like("LastName", nameWildcard)));
            }
            qry.AddOrder(Order.Asc("FirstName"));

            return qry.List<Person>();
        }
    }
}
