using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;

namespace Sample.BusinessObjects.Contacts
{
    public class Address : DataObject<Address>
    {
        public virtual int ID
        {
            get { return GetValue<int>("ID"); }
            set
            {
                BeginEdit();
                SetValue("ID", value);
            }
        }

        [ValidationNotEmpty]
        public virtual string Address1
        {
            get { return GetValue<string>("Address1"); }
            set
            {
                BeginEdit();
                SetValue("Address1", value);
            }
        }

        [ValidationNotEmpty]
        public virtual string Suburb
        {
            get { return GetValue<string>("Suburb"); }
            set
            {
                BeginEdit();
                SetValue("Suburb", value);
            }
        }

        [ValidationNotEmpty]
        public virtual string State
        {
            get { return GetValue<string>("State"); }
            set
            {
                BeginEdit();
                SetValue("State", value);
            }
        }

        public virtual string Postcode
        {
            get { return GetValue<string>("Postcode"); }
            set
            {
                BeginEdit();
                SetValue("Postcode", value);
            }
        }

        protected override void AddValidationRules()
        {
            validationRules.Add(new ValidationRule(GeneralAssertionTemplate.ValidCharacterRange(this,"Postcode","0123456789",false)));
        }
    }
}
