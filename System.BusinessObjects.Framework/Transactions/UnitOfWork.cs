using System.Collections.Generic;
using System.BusinessObjects.Providers;
using NHibernate;

namespace System.BusinessObjects.Transactions
{
    /// <summary>
    /// Represents a Unit of Work for datastore interaction
    /// </summary>
    public partial class UnitOfWork : IUnitOfWork
    {
        private ITransaction currentTransaction;
        private Queue<Action> _workQueue;

        /// <summary>
        /// Creates a new instance of Unit Of Work
        /// </summary>
        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }

        /// <summary>
        /// Creates a new instance of Unit Of Work and begins a new transaction
        /// </summary>
        public static IUnitOfWork Begin()
        {
            IUnitOfWork work = new UnitOfWork();
            work.BeginTransaction();
            return work;
        }

        /// <summary>
        /// Returns the current session
        /// </summary>
        public static ISession CurrentSession
        {
            get 
            {
                return NHibernateSessionProvider.Provider.CurrentSession;
            }
        }

        private UnitOfWork()
        {
            _workQueue = new Queue<Action>();
        }

        /// <summary>
        /// Adds a unit of work delegate to the execution stack
        /// </summary>
        public void Add(Action work)
        {
            _workQueue.Enqueue(work);
        }

        /// <summary>
        /// Executes all pending actions in the unit of work stack
        /// </summary>
        public void Execute()
        {
            try
            {
                bool shouldCommit = !IsInActiveTransaction;
                if (shouldCommit)
                    BeginTransaction();
                foreach (Action del in _workQueue)
                {
                    del();
                }
                if (shouldCommit)
                    Commit();
            }
            catch
            {
                Rollback();
                throw;
            }
        }

        #region IUnitOfWork Members

        public void Flush()
        {
            CurrentSession.Flush();
        }

        public bool IsInActiveTransaction
        {
            get { return currentTransaction != null; }
        }

        public NHibernate.ITransaction BeginTransaction()
        {
            if(IsInActiveTransaction)
                Commit();

            currentTransaction = CurrentSession.BeginTransaction();
            return currentTransaction;
        }

        public NHibernate.ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (IsInActiveTransaction)
                Commit();

            currentTransaction = CurrentSession.BeginTransaction(isolationLevel);
            return currentTransaction;
        }

        public void Commit()
        {
            ITransaction trans = currentTransaction;
            if (trans != null && trans.IsActive)
                trans.Commit();
        }

        public void Rollback()
        {
            ITransaction trans = currentTransaction;
            if (trans != null && trans.IsActive)
                trans.Rollback();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Commit();
        }

        #endregion
    }
}
