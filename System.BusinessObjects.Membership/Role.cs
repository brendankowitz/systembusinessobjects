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
    /// Role : BusinessObject
    /// </summary>
    public class Role : DataObject<Role>
    {
        public Role()
        {
            OnSaved += Role_OnSaved;
        }

        void Role_OnSaved(object sender, EventArgs e)
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

        public virtual String RoleName
        {
            get { return GetValue<String>("RoleName"); }
            set
            {
                BeginEdit();
                SetValue("RoleName", value);
                SetValue("LoweredRoleName", value.ToLower());
            }
        }

        public virtual String LoweredRoleName
        {
            get { return GetValue<String>("LoweredRoleName"); }
            set
            {
            }
        }

        public virtual String Description
        {
            get { return GetValue<String>("Description"); }
            set
            {
                BeginEdit();
                SetValue("Description", value);
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

        private ISet<User> _Users = new HashedSet<User>();
        public virtual ISet<User> Users
        {
            get
            {
                return _Users;
            }
            set
            {
                _Users = value;
            }
        }

    }
}