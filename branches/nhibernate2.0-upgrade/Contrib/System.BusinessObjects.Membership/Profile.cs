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
    public class Profile : User
    {

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

        public virtual System.Web.Profile.ProfileInfo ToProfileInfo()
        {
            System.Web.Profile.ProfileInfo retval = new System.Web.Profile.ProfileInfo(
                UserName, IsAnonymous, LastActivityDate, LastUpdatedDate, PropertyValuesBinary != null ? PropertyValuesBinary.Length : 0);
            return retval;
        }
    }
}