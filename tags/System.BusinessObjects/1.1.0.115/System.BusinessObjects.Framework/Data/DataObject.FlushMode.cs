using System;

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
