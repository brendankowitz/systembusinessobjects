using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using NUnit.Framework;
using Sample.BusinessObjects.Contacts;
using System.Diagnostics;
using NHibernate;

namespace BusinessObject.Framework.Tests
{
    [TestFixture]
    public class DataObjectTests : NHibernateInMemoryTestFixtureBase
    {
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
            p.AutoFlush = false;
            p.Save();

            Person p2 = BusinessObjectFactory.CreateAndFillPerson();
            p2.AutoFlush = false;
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
    }
}