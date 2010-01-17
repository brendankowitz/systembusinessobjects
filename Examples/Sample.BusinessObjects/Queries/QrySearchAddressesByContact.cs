using NHibernate;
using System.BusinessObjects.Data;
using Sample.BusinessObjects.Contacts;
using System.Collections.Generic;

namespace Sample.BusinessObjects.Queries
{
    public class QrySearchAddressesByContact : IDataQuery<Address>, IRequiresNHibernateSession
    {
        private readonly int _personID;
        ISession IRequiresNHibernateSession.Session { get; set; }

        public QrySearchAddressesByContact(int personID)
        {
            _personID = personID;
        }

        public IEnumerable<Address> Execute()
        {
            string sql = "select address from Person p join p.Addresses as address where p.ID = :personid";

            IQuery qry = ((IRequiresNHibernateSession)this).Session.CreateQuery(sql);
            qry.SetParameter("personid", _personID);
            qry.SetFlushMode(FlushMode.Commit);

            return qry.List<Address>();
        }

    }
}
