using System;
using Sample.BusinessObjects.Contacts;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class BusinessObjectFactory
    {
        public static void CheckPerson(Person obj, Person p2)
        {
            Assert.Equal(obj.ID, p2.ID);
            Assert.Equal(obj.FirstName, p2.FirstName);
            Assert.Equal(obj.LastName, p2.LastName);
        }

        public static Person CreateAndFillPerson()
        {
            Person p = new Person();
            p.FirstName = "John";
            p.LastName = "Smith";
            p.Email = "john@smith.com";
            return p;
        }
    }
}
