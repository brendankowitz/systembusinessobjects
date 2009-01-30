using System.BusinessObjects.Transactions;
using System.BusinessObjects.Data;
using System;

namespace System.BusinessObjects.With
{
    /// <summary>
    /// A collection of classes that can perform an action with a delegate
    /// </summary>
    public partial class With
    {
        /// <summary>
        /// With.Transaction creates and executes a transaction
        /// </summary>
        [Obsolete("Moved to System.BusinessObjects.Transactions.Transaction")]
        public class Transaction
        {
            /// <summary>
            /// Execute work within a transaction in the current session.
            /// </summary>
            public static void Execute(Action func)
            {
                Transaction.Execute(func);
            }
        }

    }
}

namespace System.BusinessObjects.Transactions
{
    /// <summary>
    /// Creates and executes work within a transaction in the current database session
    /// </summary>
    public static class Transaction
    {
        /// <summary>
        /// Saves any number of business object within a transaction in the current session.
        /// When the work is complete it is flushed to the database
        /// </summary>
        public static void Save(params DataObject[] objectsToSave)
        {
            IUnitOfWork work = UnitOfWork.Create();
            foreach (DataObject o in objectsToSave)
            {
                work.Add(o.Save);
            }
            work.Execute();
        }

        /// <summary>
        /// Execute work within a transaction in the current session.
        /// </summary>
        public static void Execute(Action func)
        {
            IUnitOfWork work = UnitOfWork.Create();
            work.Add(func);
            work.Execute();
        }
    }
}