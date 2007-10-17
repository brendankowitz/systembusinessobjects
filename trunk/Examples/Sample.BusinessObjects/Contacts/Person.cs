using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Transactions;
using Iesi.Collections.Generic;
using System.BusinessObjects.Validation;

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

        [ValidationNotEmpty]
        public virtual string FirstName
        {
            get { return GetValue<string>("FirstName"); }
            set
            {
                BeginEdit();
                SetValue("FirstName", value);
            }
        }

        [ValidationNotEmpty]
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
            UnitOfWork.CurrentSession.Flush();
        }
    }
}
