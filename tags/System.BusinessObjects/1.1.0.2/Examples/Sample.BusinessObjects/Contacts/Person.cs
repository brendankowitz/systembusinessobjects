using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Transactions;
using Iesi.Collections.Generic;
using System.BusinessObjects.Validation;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Sample.BusinessObjects.Contacts
{
    public class Person : DataObject<Person>
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
        public virtual string FirstName
        {
            get { return GetValue<string>("FirstName"); }
            set
            {
                BeginEdit();
                SetValue("FirstName", value);
            }
        }

        Regex exp = new Regex("");
        [ValidationNotEmpty]
        public virtual string LastName
        {
            get { return GetValue<string>("LastName"); }
            set
            {
                BeginEdit();
                SetValue("LastName", value);
            }
        }

        [ValidationRegex(@"[\w-]+@([\w-]+\.)+[\w-]+", ErrorMessage="{0} does not contain a valid email address")]
        public virtual string Email
        {
            get { return GetValue<string>("Email"); }
            set
            {
                BeginEdit();
                SetValue("Email", value);
            }
        }

        [NonSerialized]
        private ISet<Address> _addresses = new HashedSet<Address>();
        [XmlIgnore]
        public virtual ISet<Address> Addresses
        {
            get { return _addresses; }
            set
            {
                _addresses = value;
            }
        }
    }
}
