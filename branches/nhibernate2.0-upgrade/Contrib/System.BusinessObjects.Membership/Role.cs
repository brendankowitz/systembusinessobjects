using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// Role : BusinessObject
    /// </summary>
    public class Role : DataObject<Role>
    {

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
            }
        }

        public virtual String LoweredRoleName
        {
            get { return GetValue<String>("LoweredRoleName"); }
            set
            {
                BeginEdit();
                SetValue("LoweredRoleName", value);
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


    }
}