using System.Collections.Generic;
using System.BusinessObjects.Providers;
using NHibernate;

namespace System.BusinessObjects.Transactions
{
    public class UnitOfWork : IUnitOfWork
    {
        private ITransaction currentTransaction;
        private Queue<WorkUnitDelegate> _workQueue;

        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }

        public static ISession CurrentSession
        {
            get { return NHibernateSessionProvider.Provider.CurrentSession; }
        }

        private UnitOfWork()
        {
            _workQueue = new Queue<WorkUnitDelegate>();
        }

        public void Add(WorkUnitDelegate work)
        {
            _workQueue.Enqueue(work);
        }

        public void Execute()
        {
            try
            {
                bool shouldCommit = !IsInActiveTransaction;
                if (shouldCommit)
                    BeginTransaction();
                foreach(WorkUnitDelegate del in _workQueue)
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
            NHibernateSessionProvider.Provider.CurrentSession.Flush();
        }

        public bool IsInActiveTransaction
        {
            get { return currentTransaction != null; }
        }

        public NHibernate.ITransaction BeginTransaction()
        {
            if(IsInActiveTransaction)
                Commit();

            currentTransaction = NHibernateSessionProvider.Provider.CurrentSession.BeginTransaction();
            return currentTransaction;
        }

        public NHibernate.ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (IsInActiveTransaction)
                Commit();

            currentTransaction = NHibernateSessionProvider.Provider.CurrentSession.BeginTransaction(isolationLevel);
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
