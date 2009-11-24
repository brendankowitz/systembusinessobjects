using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using Iesi.Collections.Generic;

namespace Sample.T4BusinessObjects
{
    [Serializable]
    public partial class Customer : DataObject<Customer>
    {   
		public virtual Int32 ID
        {
            get { return GetValue<Int32>("ID"); }
            set
            {
                BeginEdit();
                SetValue("ID", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String FirstName
        {
            get { return GetValue<String>("FirstName"); }
            set
            {
                BeginEdit();
                SetValue("FirstName", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String LastName
        {
            get { return GetValue<String>("LastName"); }
            set
            {
                BeginEdit();
                SetValue("LastName", value);
            }
        }

		public virtual String Address1
        {
            get { return GetValue<String>("Address1"); }
            set
            {
                BeginEdit();
                SetValue("Address1", value);
            }
        }

		public virtual String Address2
        {
            get { return GetValue<String>("Address2"); }
            set
            {
                BeginEdit();
                SetValue("Address2", value);
            }
        }

		public virtual String Suburb
        {
            get { return GetValue<String>("Suburb"); }
            set
            {
                BeginEdit();
                SetValue("Suburb", value);
            }
        }

		public virtual String State
        {
            get { return GetValue<String>("State"); }
            set
            {
                BeginEdit();
                SetValue("State", value);
            }
        }

		public virtual String Postcode
        {
            get { return GetValue<String>("Postcode"); }
            set
            {
                BeginEdit();
                SetValue("Postcode", value);
            }
        }

		public virtual String Country
        {
            get { return GetValue<String>("Country"); }
            set
            {
                BeginEdit();
                SetValue("Country", value);
            }
        }

    }
}
