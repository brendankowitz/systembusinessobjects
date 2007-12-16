using System;
using System.Collections.Generic;
using System.Text;
using Sample.BusinessObjects.Contacts;
using NUnit.Framework;

namespace BusinessObject.Framework.Tests
{
    public class BusinessObjectFactory
    {
        public static void CheckPerson(Person obj, Person p2)
        {
            Assert.AreEqual(obj.ID, p2.ID);
            Assert.AreEqual(obj.FirstName, p2.FirstName);
            Assert.AreEqual(obj.LastName, p2.LastName);
            Assert.AreEqual(obj.RowState, p2.RowState);
        }

        public static Person CreateAndFillPerson()
        {
            Person p = new Person();
            p.FirstName = "John";
            p.LastName = "Smith";
            return p;
        }
    }
}
