using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Data
{
    public interface IDataObjectRepository<T>
        where T : DataObject
    {
        T Fetch(object Id);

        T Fetch(IDataQuery<T> query);

        IEnumerable<T> Search();

        IEnumerable<T> Search(IDataQuery<T> query);

        T Save(T src);
    }
}
