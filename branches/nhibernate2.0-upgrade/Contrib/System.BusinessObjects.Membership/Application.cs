using System;
using System.Data;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace System.BusinessObjects.Membership
{
    /// <summary>
    /// Application : BusinessObject
    /// </summary>
    public class Application : DataObject<Application>
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

        public virtual String ApplicationName
        {
            get { return GetValue<String>("ApplicationName"); }
            set
            {
                BeginEdit();
                SetValue("ApplicationName", value);
            }
        }

        public virtual String LoweredApplicationName
        {
            get { return GetValue<String>("LoweredApplicationName"); }
            set
            {
                BeginEdit();
                SetValue("LoweredApplicationName", value);
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

        public override bool Equals(object obj)
        {
            if (!(obj is Application))
                return false;
            return ID == ((Application)obj).ID;
        }

    }
}