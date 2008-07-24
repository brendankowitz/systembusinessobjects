using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// Paths : BusinessObject
    /// </summary>
    public class Paths : DataObject<Paths>
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

        public virtual String Path
        {
            get { return GetValue<String>("Path"); }
            set
            {
                BeginEdit();
                SetValue("Path", value);
            }
        }

        public virtual String LoweredPath
        {
            get { return GetValue<String>("LoweredPath"); }
            set
            {
                BeginEdit();
                SetValue("LoweredPath", value);
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