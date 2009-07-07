using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace Sample.BusinessObjects.Contacts
{
    [Serializable]
    public class PersonType : DataObject<PersonType>
    {
        [ValidationNotEmpty]
        public virtual string ID
        {
            get { return GetValue(p => p.ID); }
            set
            {
                BeginEdit();
                SetValue(p => p.ID, value);
            }
        }

        [ValidationNotEmpty]
        public virtual string Description
        {
            get { return GetValue(p => p.Description); }
            set
            {
                BeginEdit();
                SetValue(p => p.Description, value);
            }
        }
    }
}
