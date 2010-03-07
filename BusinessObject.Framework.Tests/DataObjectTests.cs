using System;
using System.Data;
using Sample.BusinessObjects.Contacts;
using System.Diagnostics;
using System.ComponentModel;
using System.BusinessObjects.Data;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace BusinessObject.Framework.Tests
{
    public class DataObjectTests : NHibernateInMemoryTestFixtureBase
    {
        IDataObjectRepository<Person> _repository;

        public DataObjectTests()
        {
            _repository = new NHExpressionsRepository<Person>(session);
        }

        [Fact]
        public void GenericTypeTest()
        {
            Type t = typeof(DataObject<Person>);

            Type t2 = Type.GetType("System.BusinessObjects.Data.DataObject`1[[Sample.BusinessObjects.Contacts.Person, Sample.BusinessObjects]], System.BusinessObjects.Framework");

            Assert.Equal(t, t2);
        }

        #region RowStateTests
        
        [Fact]
        public void CheckRowStateDetached()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            //Assert.Equal(DataRowState.Detached, p.RowState);

            var repository = new NHExpressionsRepository<Person>(session);
            Assert.True(p.IsDirty);

            repository.Save(p);

            Assert.False(p.IsDirty);
        }

        
        [Fact]
        public void CheckRowStateSaved()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            IDataObjectRepository<Person> repository = new NHExpressionsRepository<Person>(session);
            repository.Save(p);
            Assert.False(p.IsDirty);
        }

        /*
        [Fact]
        public void CheckRowStateLoaded()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            //p.Save();
            //Person p2 = Person.Load(p.ID);
            //Assert.Equal(DataRowState.Unchanged, p2.RowState);
        }

        [Fact]
        public void CheckRowStateModified()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            Person p2 = Person.Load(p.ID);

            p2.FirstName = "Jenny";

            Assert.Equal(DataRowState.Modified, p2.RowState);
        }

        [Fact]
        public void CheckRowStateSetModified()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();

            Person p2 = BusinessObjectFactory.CreateAndFillPerson();
            p2.ID = p.ID;
            p2.RowState = DataRowState.Modified;

            Assert.Equal(DataRowState.Modified, p2.RowState);
        }

        [Fact]
        public void CheckRowStateSetModifiedSaved()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();

            Person p2 = BusinessObjectFactory.CreateAndFillPerson();
            p2.ID = p.ID;
            p2.RowState = DataRowState.Modified;

            session.Evict(p);

            //Person.Evict(p.ID);

            //ITransaction t = session.BeginTransaction();

            p2.Save();

            Assert.Equal(DataRowState.Unchanged, p2.RowState);

            //t.Rollback();
        }

        [Fact]
        public void CheckRowStateDeleted()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            p.Delete();
            Assert.Equal(DataRowState.Deleted, p.RowState);

            p.Save();
            Trace.WriteLine(p.RowState);
            Assert.Equal(DataRowState.Deleted, p.RowState);
        }
         * */
        #endregion

        #region Validation Tests
        [Fact]
        public void TestStringEmptyValidation_NullString()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = null;

            Assert.True(p.IsNull("FirstName"));

            IDataErrorInfo error = p;
            Assert.NotEmpty(error["FirstName"]);
        }

        [Fact]
        public void TestStringEmptyValidation_NullString_LambdaIsNull()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = null;

            Assert.True(p.IsNull(pr => pr.FirstName));

            IDataErrorInfo error = p;
            Assert.NotEmpty(error["FirstName"]);
        }

        [Fact]
        public void TestStringEmptyValidation_EmptyString()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = string.Empty;

            Assert.False(p.IsNull("FirstName"));

            IDataErrorInfo error = p;
            Assert.NotEmpty(error["FirstName"]);
        }

        [Fact]
        public void TestNullValidationAttibute()
        {
            Address a = new Address();
            Assert.True(a.IsNull("Postcode"));
            IDataErrorInfo error = a;
            Trace.WriteLine(error["Postcode"]);
            Assert.NotEmpty(error["Postcode"]);

            a.Postcode = "1234";
            Assert.Empty(error["Postcode"]);

        }

        [Fact]
        public void TestInvalidRange()
        {
            Address a = new Address();
            Assert.True(a.IsNull("Postcode"));
            
            IDataErrorInfo error = a;
            Trace.WriteLine(error["Postcode"]);
            Assert.NotEmpty(error["Postcode"]);

            a.Postcode = "1234a";
            Assert.NotEmpty(error["Postcode"]);

        }

        //[Fact]
        //public void TestNHibernateValidationWhenSaving()
        //{
        //    Address a = new Address();
        //    Assert.True(a.IsNull("Postcode"));

        //    IDataErrorInfo error = a;
        //    Trace.WriteLine(error["Postcode"]);
        //    Assert.NotEmpty(error["Postcode"]);

        //    Assert.Throws<NHibernate.Classic.ValidationFailure>(() => {a.Save();});
        //}

        //[Fact]
        //public void TestNHibernateValidationWhenSaving2()
        //{
        //    Address a = new Address();
        //    Assert.True(a.IsNull("Postcode"));
        //    a.Postcode = "1234";
        //    a.Address1 = "Address1";
        //    a.Suburb = "Suburb";
        //    a.State = "QLD";
        //    a.Save();

        //    a.Postcode = null;
        //    Assert.Throws<NHibernate.Classic.ValidationFailure>(() => { a.Save(SaveMode.Flush); });
        //}

        //[Fact]
        //public void TestNHibernateValidationWhenDeleting()
        //{
        //    Address a = new Address();
        //    Assert.True(a.IsNull("Postcode"));
        //    a.Postcode = "1234";
        //    a.Address1 = "Address1";
        //    a.Suburb = "Suburb";
        //    a.State = "QLD";
        //    a.Save();

        //    a.Postcode = null;
        //    a.Delete();
        //    a.Save();

        //}

        [Fact]
        public void TestNHibernateValidationRegex()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            IDataErrorInfo error = p;

            Assert.Empty(error["Email"]);

            p.Email = "invalid.email";

            Trace.WriteLine(error["Email"]);
            Assert.NotEmpty(error["Email"]);
        }

        [Fact]
        public void TestNHibernateValidationRegexNullProperty()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            IDataErrorInfo error = p;

            Assert.Empty(error["Email"]);

            p.Email = null;

            Trace.WriteLine(error["Email"]);
            Assert.NotEmpty(error["Email"]);
        }

        [Fact]
        public void ValidationFromDotNet35DataAnnotationAttribute()
        {
            DataAnnotationsTestClass obj = new DataAnnotationsTestClass();
            obj.IntRange = 0;
            Assert.Empty(((IDataErrorInfo)obj)["IntRange"]);

            obj.IntRange = 10;
            Assert.Equal("The field IntRange must be between 0 and 5.", ((IDataErrorInfo)obj)["IntRange"]);
        }

        #endregion

        #region Property Changed Events

        [Fact]
        public void PropertyChanging()
        {
            bool beforeEvent = false;
            bool afterEvent = false;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.PropertyChanging += (sender, e) =>
            {
                beforeEvent = true;
                Assert.Equal("John", p.FirstName);
                Assert.Equal("FirstName", e.PropertyName);
            };
            p.PropertyChanged += (sender, e) =>
            {
                afterEvent = true;
                Assert.Equal("Peter", p.FirstName);
                Assert.Equal("FirstName", e.PropertyName);
            };

            p.FirstName = "Peter";

            Assert.Equal("Peter", p.FirstName);
            Assert.Equal(true, beforeEvent);
            Assert.Equal(true, afterEvent);
        }

        #endregion

        #region GetValue

        [Fact]
        public void GetPrimitive()
        {
            PrimitiveTestClass c = new PrimitiveTestClass();

            Assert.Equal<ulong>(0, c.unsignedLong);
            Assert.Equal(0, c.integer);
            Assert.Equal(0, c.character);
            Assert.Equal(false, c.boolean);
        }

        #endregion

        #region Serialising

        //[Fact]
        //public void TestSerialise()
        //{
        //    Person p = BusinessObjectFactory.CreateAndFillPerson();
        //    p.ID = 21;

        //    Person newPerson = p.Clone();

        //    Assert.Equal(p.ID, newPerson.ID);
        //}

        //[Fact]
        //public void TestSerialiseLazyManyToOne()
        //{
        //    Person p = BusinessObjectFactory.CreateAndFillPerson();
        //    Address addr = new Address
        //    {
        //        Address1 = "12 Street",
        //        Suburb = "Suburb",
        //        State = "QLD",
        //        Postcode = "1234"
        //    };
        //    p.Addresses.Add(addr);

        //    PersonType ptype = new PersonType
        //    {
        //        ID = "contact",
        //        Description = "General Contact"
        //    };
        //    ptype.Save(SaveMode.Flush);
        //    p.ContactType = ptype;
        //    p.Save(SaveMode.Flush);

        //    p.Evict();
        //    ptype.Evict();

        //    Person loadedPerson = Person.Load(p.ID, session);
        //    Person newPerson = loadedPerson.Clone();

        //    Assert.Equal(p.ID, newPerson.ID);
        //    Assert.Equal("contact", newPerson.ContactType.ID);
        //}

        //[Fact]
        //public void TestSerialiseLazyCollection()
        //{
        //    Person p = BusinessObjectFactory.CreateAndFillPerson();
        //    Address addr = new Address
        //    {
        //        Address1 = "12 Street",
        //        Suburb = "Suburb",
        //        State = "QLD",
        //        Postcode = "1234"
        //    };
        //    p.Addresses.Add(addr);

        //    PersonType ptype = new PersonType
        //    {
        //        ID = "contact",
        //        Description = "General Contact"
        //    };
        //    ptype.Save(SaveMode.Flush);
        //    p.ContactType = ptype;
        //    p.Save(SaveMode.Flush);

        //    p.Evict();
        //    ptype.Evict();
        //    addr.Evict();

        //    Person loadedPerson = Person.Load(p.ID, session);
        //    Person newPerson = loadedPerson.Clone();
        //    loadedPerson.Evict();

        //    //the only way so far I can see is to re-attach collections is to lock/attach the base entity
        //    session.Lock(newPerson, NHibernate.LockMode.None);

        //    int addrCount = newPerson.Addresses.Count;

        //    Assert.Equal(p.ID, newPerson.ID);
        //    Assert.Equal("contact", newPerson.ContactType.ID);
        //    Assert.Equal(1, addrCount);
        //}

        #endregion

        #region TestClass
        public class DataAnnotationsTestClass : DataObject
        {
            [Range(0,5)]
            public virtual int IntRange
            {
                get { return GetValue<int>("IntRange"); }
                set
                {
                    BeginEdit();
                    SetValue("IntRange", value);
                }
            }
        }

        public class PrimitiveTestClass : DataObject
        {
            public virtual ulong unsignedLong
            {
                get { return GetValue<ulong>("unsignedLong"); }
                set
                {
                    BeginEdit();
                    SetValue("unsignedLong", value);
                }
            }

            public virtual int integer
            {
                get { return GetValue<int>("integer"); }
                set
                {
                    BeginEdit();
                    SetValue("integer", value);
                }
            }

            public virtual bool boolean
            {
                get { return GetValue<bool>("boolean"); }
                set
                {
                    BeginEdit();
                    SetValue("boolean", value);
                }
            }

            public virtual char character
            {
                get { return GetValue<char>("character"); }
                set
                {
                    BeginEdit();
                    SetValue("character", value);
                }
            }
        }
        #endregion
    }
}
