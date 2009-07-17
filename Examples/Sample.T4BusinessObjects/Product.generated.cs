using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using Iesi.Collections.Generic;

namespace Sample.T4BusinessObjects
{
    [Serializable]
    public partial class Product : DataObject<Product>
    {   
		public virtual Int32 ID
        {
            get { return GetValue<Int32>("ID"); }
            set
            {
                BeginEdit();
                SetValue("ID", value);
            }
        }

		[ValidationIsNotNull]
		public virtual String Name
        {
            get { return GetValue<String>("Name"); }
            set
            {
                BeginEdit();
                SetValue("Name", value);
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

		public virtual Decimal Cost
        {
            get { return GetValue<Decimal>("Cost"); }
            set
            {
                BeginEdit();
                SetValue("Cost", value);
            }
        }

		public virtual Int32 Quantity
        {
            get { return GetValue<Int32>("Quantity"); }
            set
            {
                BeginEdit();
                SetValue("Quantity", value);
            }
        }

    }
}
