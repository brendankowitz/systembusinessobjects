using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// Profile : BusinessObject
    /// </summary>
    public class Profile : DataObject<Profile>
    {
        public Profile()
        {
            AutoFlush = false;
        }

        public virtual Guid ID
        {
            get { return GetValue<Guid>("ID"); }
            set
            {
                BeginEdit();
                SetValue("ID", value);
            }
        }

        public virtual String PropertyNames
        {
            get { return GetValue<String>("PropertyNames"); }
            set
            {
                BeginEdit();
                SetValue("PropertyNames", value);
            }
        }

        public virtual String PropertyValuesString
        {
            get { return GetValue<String>("PropertyValuesString"); }
            set
            {
                BeginEdit();
                SetValue("PropertyValuesString", value);
            }
        }

        public virtual Byte[] PropertyValuesBinary
        {
            get { return GetValue<Byte[]>("PropertyValuesBinary"); }
            set
            {
                BeginEdit();
                SetValue("PropertyValuesBinary", value);
            }
        }

        public virtual DateTime LastUpdatedDate
        {
            get { return GetValue<DateTime>("LastUpdatedDate"); }
            set
            {
                BeginEdit();
                SetValue("LastUpdatedDate", value);
            }
        }

        private User _user;
        public virtual User User
        {
            get
            {
                return _user;
            }
            set
            {
                _user = value;
            }
        }

    }
}