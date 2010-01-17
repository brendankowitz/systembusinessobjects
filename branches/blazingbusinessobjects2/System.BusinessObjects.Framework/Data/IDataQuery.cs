using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Data
{
    public interface IDataQuery<TResult>
    {
        IEnumerable<TResult> Execute();
    }
}
