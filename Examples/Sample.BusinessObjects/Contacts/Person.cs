using System;
using System.BusinessObjects.Data;
using Iesi.Collections.Generic;
using System.BusinessObjects.Validation;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Sample.BusinessObjects.Contacts
{
    [Serializable]
    public class Person : DataObject<Person>
    {
        public virtual int ID
        {
            get { return GetValue(p => p.ID); }
            set
            {
                BeginEdit();
                SetValue(p => p.ID, value);
            }
        }

        [Required]
        public virtual string FirstName
        {
            get { return GetValue(p => p.FirstName); }
            set
            {
                BeginEdit();
                SetValue(p => p.FirstName, value);
            }
        }

        [Required]
        public virtual string LastName
        {
            get { return GetValue(p => p.LastName); }
            set
            {
                BeginEdit();
                SetValue(p => p.LastName, value);
            }
        }

        [Required]
        [RegularExpression(@"[\w-]+@([\w-]+\.)+[\w-]+", ErrorMessage="Please provide a valid email address")]
        public virtual string Email
        {
            get { return GetValue(p => p.Email); }
            set
            {
                BeginEdit();
                SetValue(p => p.Email, value);
            }
        }

        private ISet<Address> _addresses = new HashedSet<Address>();
        [XmlIgnore]
        public virtual ISet<Address> Addresses
        {
            get 
            {
                return _addresses; 
            }
            set
            {
                _addresses = value;
            }
        }

        private PersonType _contactType = null;
        public virtual PersonType ContactType
        {
            get
            {
                return _contactType;
            }
            set
            {
                _contactType = value;
            }
        }
    }
}
