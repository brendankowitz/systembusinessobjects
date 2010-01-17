using System;
using NHibernate;

namespace System.BusinessObjects.Data
{
    public interface IRequiresNHibernateSession
    {
        ISession Session { set; get; }
    }
}
