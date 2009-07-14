using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using Iesi.Collections.Generic;

namespace Sample.T4BusinessObjects
{
    [Serializable]
    public partial class Customer : DataObject<Customer>
    {   
		[ValidationIsNotNull]
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

		[ValidationIsNotNull]
		public virtual String Address1
        {
            get { return GetValue<String>("Address1"); }
            set
            {
                BeginEdit();
                SetValue("Address1", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String Address2
        {
            get { return GetValue<String>("Address2"); }
            set
            {
                BeginEdit();
                SetValue("Address2", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String Suburb
        {
            get { return GetValue<String>("Suburb"); }
            set
            {
                BeginEdit();
                SetValue("Suburb", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String State
        {
            get { return GetValue<String>("State"); }
            set
            {
                BeginEdit();
                SetValue("State", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String Postcode
        {
            get { return GetValue<String>("Postcode"); }
            set
            {
                BeginEdit();
                SetValue("Postcode", value);
            }
        }

		[ValidationIsNotNull]
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
