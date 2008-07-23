using NHibernate;
using NHibernate.Expression;
using Sample.BusinessObjects.Contacts;
using System.BusinessObjects.Transactions;

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
            ICriteria qry = UnitOfWork.CurrentSession.CreateCriteria(typeof(Person));

            if(!string.IsNullOrEmpty(name))
            {
                string nameWildcard = string.Format("%{0}%", name);
                qry.Add(Expression.Or(Expression.Like("FirstName", nameWildcard), Expression.Like("LastName", nameWildcard)));
            }

            qry.AddOrder(Order.Asc("FirstName"));

            return qry;
        }
    }
}
