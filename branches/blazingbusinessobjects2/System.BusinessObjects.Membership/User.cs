using System;
using System.BusinessObjects.Data;
using Iesi.Collections.Generic;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// User : BusinessObject
    /// </summary>
    public class User : DataObject<User>
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

        private Profile _profile;
        public virtual Profile Profile
        {
            get
            {
                return _profile;
            }
            set
            {
                _profile = value;
                if (value != null && !value.IsDeleted)
                    value.User = this;
            }
        }

        public virtual System.Web.Profile.ProfileInfo ToProfileInfo()
        {
            System.Web.Profile.ProfileInfo retval = new System.Web.Profile.ProfileInfo(
                UserName,
                IsAnonymous,
                LastActivityDate,
                Profile.LastUpdatedDate,
                Profile.PropertyValuesBinary != null ? Profile.PropertyValuesBinary.Length : 0);
            return retval;
        }

        private ISet<Role> _roles = new HashedSet<Role>();
        public virtual ISet<Role> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }
    }
}