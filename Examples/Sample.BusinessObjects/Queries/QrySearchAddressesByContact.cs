using NHibernate;

using System.BusinessObjects.Providers;

namespace Sample.BusinessObjects.Queries
{
    public class QrySearchAddressesByContact
    {
        public static IQuery Query(int personID)
        {
            string sql = "select address from Person p join p.Addresses as address where p.ID = :personid";

            IQuery qry = NHibernateSessionProvider.Provider.CurrentSession.CreateQuery(sql);
            qry.SetParameter("personid", personID);

            return qry;
        }
    }
}
