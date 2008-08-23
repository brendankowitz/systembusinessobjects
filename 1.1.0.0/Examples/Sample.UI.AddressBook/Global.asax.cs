using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Providers;

namespace Sample.UI.AddressBook
{
    public class Global : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();

            EndRequest += Global_EndRequest;
            BeginRequest += Global_BeginRequest;

        }

        void Global_BeginRequest(object sender, EventArgs e)
        {
            ((NHibernateAspContextProvider)NHibernateSessionProvider.Provider).BindNewSession();
        }

        static void Global_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionProvider.Provider.CloseSession();
        }
    }
}