using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using System.BusinessObjects.Transactions;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// User : BusinessObject
    /// </summary>
    public class User : DataObject<User>
    {
        public User()
        {
            OnSaved += User_OnSaved;
        }

        void User_OnSaved(object sender, EventArgs e)
        {
            //Flush all saves from this object to the db.
            UnitOfWork.CurrentSession.Flush();
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

        public virtual String UserName
        {
            get { return GetValue<String>("UserName"); }
            set
            {
                BeginEdit();
                SetValue("UserName", value);
                SetValue("LoweredUserName", value.ToLower());
            }
        }

        public virtual String LoweredUserName
        {
            get { return GetValue<String>("LoweredUserName"); }
            set
            {
            }
        }

        public virtual String MobileAlias
        {
            get { return GetValue<String>("MobileAlias"); }
            set
            {
                BeginEdit();
                SetValue("MobileAlias", value);
            }
        }

        public virtual Boolean IsAnonymous
        {
            get { return GetValue<Boolean>("IsAnonymous"); }
            set
            {
                BeginEdit();
                SetValue("IsAnonymous", value);
            }
        }

        public virtual DateTime LastActivityDate
        {
            get { return GetValue<DateTime>("LastActivityDate"); }
            set
            {
                BeginEdit();
                SetValue("LastActivityDate", value);
            }
        }

        protected Application _Application;
        public virtual Application Application
        {
            get
            {
                return _Application;
            }
            set
            {
                BeginEdit();
                _Application = value;
            }
        }
    }
}