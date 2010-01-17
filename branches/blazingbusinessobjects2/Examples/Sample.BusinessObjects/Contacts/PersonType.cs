using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using System.ComponentModel.DataAnnotations;

namespace Sample.BusinessObjects.Contacts
{
    [Serializable]
    public class PersonType : DataObject<PersonType>
    {
        [Required]
        public virtual string ID
        {
            get { return GetValue(p => p.ID); }
            set
            {
                BeginEdit();
                SetValue(p => p.ID, value);
            }
        }

        [Required]
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
