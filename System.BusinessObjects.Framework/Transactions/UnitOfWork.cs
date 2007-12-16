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
        private Queue<WorkUnitDelegate> _workQueue;
        //private static IWindsorContainer container;
        //private static readonly object containerSync = new object();

        /// <summary>
        /// Creates a new instance of Unit Of Work
        /// </summary>
        public static IUnitOfWork Create()
        {
            return new UnitOfWork();
        }

        ///// <summary>
        ///// Returns a static Windsor IoC container
        ///// </summary>
        //public static IWindsorContainer IoC
        //{
        //    get
        //    {
        //        if (container == null)
        //        {
        //            lock (containerSync)
        //            {
        //                container = new WindsorContainer(new Castle.Windsor.Configuration.Interpreters.XmlInterpreter());
        //            }
        //        }
        //        return container;
        //    }
        //}

        /// <summary>
        /// Returns the current session
        /// </summary>
        public static ISession CurrentSession
        {
            get 
            {
                //NHibernateSessionProvider provider = IoC.Resolve(typeof(NHibernateSessionProvider)) as NHibernateSessionProvider;
                //if(provider == null)
                //    provider = NHibernateSessionProvider.Provider;
                return NHibernateSessionProvider.Provider.CurrentSession;
            }
        }

        private UnitOfWork()
        {
            _workQueue = new Queue<WorkUnitDelegate>();
        }

        /// <summary>
        /// Adds a unit of work delegate to the execution stack
        /// </summary>
        public void Add(WorkUnitDelegate work)
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
