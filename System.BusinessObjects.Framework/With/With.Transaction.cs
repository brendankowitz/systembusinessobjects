using System.BusinessObjects.Transactions;

namespace System.BusinessObjects.With
{
    public partial class With
    {
        public class Transaction
        {
            public static void Execute(WorkUnitDelegate func)
            {
                IUnitOfWork work = UnitOfWork.Create();
                work.Add(func);
                work.Execute();
                work.Flush();
            }
        }
        
    }
}
