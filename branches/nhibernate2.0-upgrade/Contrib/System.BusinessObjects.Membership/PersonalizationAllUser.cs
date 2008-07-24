using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// PersonalizationAllUser : BusinessObject
    /// </summary>
    public class PersonalizationAllUser : DataObject<PersonalizationAllUser>
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

        public virtual Byte[] PageSettings
        {
            get { return GetValue<Byte[]>("PageSettings"); }
            set
            {
                BeginEdit();
                SetValue("PageSettings", value);
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

        protected Paths _Path;
        public virtual Paths Path
        {
            get
            {
                return _Path;
            }
            set
            {
                BeginEdit();
                _Path = value;
            }
        }


    }
}