using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.BusinessObjects.Data
{
    public enum SaveMode
    {
        /// <summary>
        /// Flush changes immediately after Save
        /// </summary>
        Flush,
        /// <summary>
        /// Use NHibernate configuration
        /// </summary>
        Auto
    }
}
