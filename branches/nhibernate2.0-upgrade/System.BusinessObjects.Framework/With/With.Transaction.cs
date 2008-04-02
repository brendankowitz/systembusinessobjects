using System.BusinessObjects.Transactions;

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
        public class Transaction
        {
            /// <summary>
            /// Execute work within a transaction in the current session.
            /// </summary>
            public static void Execute(WorkUnitDelegate func)
            {
                IUnitOfWork work = UnitOfWork.Create();
                work.Add(func);
                work.Execute();
            }
        }
        
    }
}
