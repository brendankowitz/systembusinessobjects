using System;
using System.BusinessObjects.Data;
using System.BusinessObjects.Validation;
using Iesi.Collections.Generic;

namespace Sample.T4BusinessObjects
{
    [Serializable]
    public partial class Order : DataObject<Order>
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

		public virtual Decimal Cost
        {
            get { return GetValue<Decimal>("Cost"); }
            set
            {
                BeginEdit();
                SetValue("Cost", value);
            }
        }

		public virtual Customer Customer
        {
            get;
            set;
        }

		ISet<Product> _Products = new HashedSet<Product>();
		public virtual ISet<Product> Products
        {
            get{ return _Products; }
            set{ _Products = value; }
        }

    }
}
