using System;
using System.BusinessObjects.Providers;

namespace Sample.UI.AddressBook
{
    public class Global : System.Web.HttpApplication
    {
        public override void Init()
        {
            base.Init();
            EndRequest += Global_EndRequest;
        }

        static void Global_EndRequest(object sender, EventArgs e)
        {
            NHibernateSessionProvider.Provider.CloseSession();
        }
    }
}