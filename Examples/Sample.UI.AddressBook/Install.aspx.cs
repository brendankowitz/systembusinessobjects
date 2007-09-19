using System;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace Sample.UI.AddressBook
{
    public partial class Install : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ButtonInstall_Click(object sender, EventArgs e)
        {
            Configuration cfg = new Configuration();
            cfg.Configure();
            cfg.BuildSessionFactory();
            SchemaExport ex = new SchemaExport(cfg);
            ex.Execute(true, true, false, false);
        }
    }
}
