using System;
using System.Collections.Generic;
using System.Text;
using Sample.BusinessObjects.Contacts;

namespace BusinessObject.Framework.Tests
{
    public class BusinessObjectFactory
    {
        public static Person CreateAndFillPerson()
        {
            Person p = new Person();
            p.FirstName = "John";
            p.LastName = "Smith";
            return p;
        }
    }
}
