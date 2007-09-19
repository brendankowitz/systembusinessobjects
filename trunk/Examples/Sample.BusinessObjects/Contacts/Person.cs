using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Providers;
using System.Data;
using Iesi.Collections.Generic;

namespace Sample.BusinessObjects.Contacts
{
    public class Person : DataObject<Person>
    {
        public virtual int ID
        {
            get { return GetValue<int>("ID"); }
            set
            {
                BeginEdit();
                SetValue("ID", value);
            }
        }

        public virtual string FirstName
        {
            get { return GetValue<string>("FirstName"); }
            set
            {
                BeginEdit();
                SetValue("FirstName", value);
            }
        }

        public virtual string LastName
        {
            get { return GetValue<string>("LastName"); }
            set
            {
                BeginEdit();
                SetValue("LastName", value);
            }
        }

        [NonSerialized]
        private ISet<Address> _addresses = new HashedSet<Address>();
        public virtual ISet<Address> Addresses
        {
            get { return _addresses; }
            set
            {
                _addresses = value;
            }
        }

        public override void Save()
        {
            base.Save();

            //Flush so the Feature ForiegnKeys are saved.
            NHibernateSessionProvider.Provider.CurrentSession.Flush();
        }
    }
}
