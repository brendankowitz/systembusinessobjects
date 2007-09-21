using NHibernate;
using System.BusinessObjects.Providers;
using Sample.BusinessObjects.Contacts;
using NHibernate.Expression;

namespace Sample.BusinessObjects.Queries
{
    /// <summary>
    /// Provides a query to search for contacts by name
    /// </summary>
    public class QrySearchContactByName
    {
        /// <summary>
        /// Query
        /// </summary>
        /// <param name="name">Name of the contact to search for</param>
        public static ICriteria Query(string name)
        {
            ICriteria qry = NHibernateSessionProvider.Provider.CurrentSession.CreateCriteria(typeof(Person));

            if(!string.IsNullOrEmpty(name))
            {
                string nameWildcard = string.Format("%{0}%", name);
                qry.Add(Expression.Or(Expression.Like("FirstName", nameWildcard), Expression.Like("LastName", nameWildcard)));
            }

            qry.AddOrder(Order.Asc("FirstName"));
            qry.SetCacheable(false);

            return qry;
        }
    }
}
