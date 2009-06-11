using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;
using System.Diagnostics;
using NHibernate;
using System.ComponentModel;
using System.BusinessObjects.Data;
using System.ComponentModel.DataAnnotations;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class DataObjectTests : NHibernateInMemoryTestFixtureBase
    {
        [Test]
        public void GenericTypeTest()
        {
            Type t = typeof(DataObject<Person>);

            Type t2 = Type.GetType("System.BusinessObjects.Data.DataObject`1[[Sample.BusinessObjects.Contacts.Person, Sample.BusinessObjects]], System.BusinessObjects.Framework");

            Assert.AreEqual(t, t2);
        }

        #region RowStateTests
        [Test]
        public void CheckRowStateDetached()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            Assert.AreEqual(DataRowState.Detached, p.RowState);
        }

        [Test]
        public void CheckRowStateSaved()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            Assert.AreEqual(DataRowState.Unchanged, p.RowState);
        }

        [Test]
        public void CheckRowStateLoaded()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            Person p2 = Person.Load(p.ID);
            Assert.AreEqual(DataRowState.Unchanged, p2.RowState);
        }

        [Test]
        public void CheckRowStateModified()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            Person p2 = Person.Load(p.ID);

            p2.FirstName = "Jenny";

            Assert.AreEqual(DataRowState.Modified, p2.RowState);
        }

        [Test]
        public void CheckRowStateSetModified()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();

            Person p2 = BusinessObjectFactory.CreateAndFillPerson();
            p2.ID = p.ID;
            p2.RowState = DataRowState.Modified;

            Assert.AreEqual(DataRowState.Modified, p2.RowState);
        }

        [Test]
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

            Assert.AreEqual(DataRowState.Unchanged, p2.RowState);

            //t.Rollback();
        }

        [Test]
        public void CheckRowStateDeleted()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.Save();
            p.Delete();
            Assert.AreEqual(DataRowState.Deleted, p.RowState);

            p.Save();
            Trace.WriteLine(p.RowState);
            Assert.AreEqual(DataRowState.Deleted, p.RowState);
        }
        #endregion

        #region Validation Tests
        [Test]
        public void TestStringEmptyValidation_NullString()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = null;

            Assert.IsTrue(p.IsNull("FirstName"));

            IDataErrorInfo error = p;
            Assert.IsNotEmpty(error["FirstName"]);
        }

        [Test]
        public void TestStringEmptyValidation_NullString_LambdaIsNull()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = null;

            Assert.IsTrue(p.IsNull(pr => pr.FirstName));

            IDataErrorInfo error = p;
            Assert.IsNotEmpty(error["FirstName"]);
        }

        [Test]
        public void TestStringEmptyValidation_EmptyString()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.FirstName = string.Empty;

            Assert.IsFalse(p.IsNull("FirstName"));

            IDataErrorInfo error = p;
            Assert.IsNotEmpty(error["FirstName"]);
        }

        [Test]
        public void TestNullValidationAttibute()
        {
            Address a = new Address();
            Assert.IsTrue(a.IsNull("Postcode"));
            IDataErrorInfo error = a;
            Trace.WriteLine(error["Postcode"]);
            Assert.IsNotEmpty(error["Postcode"]);

            a.Postcode = "1234";
            Assert.IsEmpty(error["Postcode"]);

        }

        [Test]
        public void TestInvalidRange()
        {
            Address a = new Address();
            Assert.IsTrue(a.IsNull("Postcode"));
            
            IDataErrorInfo error = a;
            Trace.WriteLine(error["Postcode"]);
            Assert.IsNotEmpty(error["Postcode"]);

            a.Postcode = "1234a";
            Assert.IsNotEmpty(error["Postcode"]);

        }

        [Test]
        [ExpectedException(typeof(NHibernate.Classic.ValidationFailure))]
        public void TestNHibernateValidationWhenSaving()
        {
            Address a = new Address();
            Assert.IsTrue(a.IsNull("Postcode"));

            IDataErrorInfo error = a;
            Trace.WriteLine(error["Postcode"]);
            Assert.IsNotEmpty(error["Postcode"]);

            a.Save();

        }

        [Test]
        [ExpectedException(typeof(NHibernate.Classic.ValidationFailure))]
        public void TestNHibernateValidationWhenSaving2()
        {
            Address a = new Address();
            Assert.IsTrue(a.IsNull("Postcode"));
            a.Postcode = "1234";
            a.Address1 = "Address1";
            a.Suburb = "Suburb";
            a.State = "QLD";
            a.Save();

            a.Postcode = null;
            a.Save(SaveMode.Flush);
        }

        [Test]
        public void TestNHibernateValidationWhenDeleting()
        {
            Address a = new Address();
            Assert.IsTrue(a.IsNull("Postcode"));
            a.Postcode = "1234";
            a.Address1 = "Address1";
            a.Suburb = "Suburb";
            a.State = "QLD";
            a.Save();

            a.Postcode = null;
            a.Delete();
            a.Save();

        }

        [Test]
        public void TestNHibernateValidationRegex()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            IDataErrorInfo error = p;

            Assert.IsEmpty(error["Email"]);

            p.Email = "invalid.email";

            Trace.WriteLine(error["Email"]);
            Assert.IsNotEmpty(error["Email"]);
        }

        [Test]
        public void TestNHibernateValidationRegexNullProperty()
        {
            Person p = BusinessObjectFactory.CreateAndFillPerson();
            IDataErrorInfo error = p;

            Assert.IsEmpty(error["Email"]);

            p.Email = null;

            Trace.WriteLine(error["Email"]);
            Assert.IsNotEmpty(error["Email"]);
        }

        [Test]
        public void ValidationFromDotNet35DataAnnotationAttribute()
        {
            DataAnnotationsTestClass obj = new DataAnnotationsTestClass();
            obj.IntRange = 0;
            Assert.IsEmpty(((IDataErrorInfo)obj)["IntRange"]);

            obj.IntRange = 10;
            Assert.AreEqual("The field IntRange must be between 0 and 5.", ((IDataErrorInfo)obj)["IntRange"]);
        }

        #endregion

        #region Property Changed Events

        [Test]
        public void PropertyChanging()
        {
            bool beforeEvent = false;
            bool afterEvent = false;

            Person p = BusinessObjectFactory.CreateAndFillPerson();
            p.PropertyChanging += delegate(object sender, PropertyChangingEventArgs e)
            {
                beforeEvent = true;
                Assert.AreEqual("John", p.FirstName);
                Assert.AreEqual("FirstName", e.PropertyName);
            };
            p.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
            {
                afterEvent = true;
                Assert.AreEqual("Peter", p.FirstName);
                Assert.AreEqual("FirstName", e.PropertyName);
            };

            p.FirstName = "Peter";

            Assert.AreEqual("Peter", p.FirstName);
            Assert.AreEqual(true, beforeEvent);
            Assert.AreEqual(true, afterEvent);
        }

        #endregion

        #region GetValue

        [Test]
        public void GetPrimitive()
        {
            PrimitiveTestClass c = new PrimitiveTestClass();

            Assert.AreEqual(0, c.unsignedLong);
            Assert.AreEqual(-1, c.integer);
            Assert.AreEqual(0, c.character);
            Assert.AreEqual(false, c.boolean);
        }

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
