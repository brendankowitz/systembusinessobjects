using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Data
{
    /// <summary>
    /// A IDataObjectRepository{T} repository that will automatically submit changes
    /// on Dispose
    /// </summary>
    public interface IScopedRepository<T> : IDataObjectRepository<T>, IDisposable
        where T : DataObject
    {

    }
}
