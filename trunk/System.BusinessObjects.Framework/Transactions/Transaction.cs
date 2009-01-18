using System.BusinessObjects.Transactions;
using System.BusinessObjects.Data;

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
            work.Add(() => o.Save());
        }
        work.Execute();
    }

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