using System;

namespace System.BusinessObjects.MethodLinq
{
    /// <summary>
    /// Provides access to IQueryable based on methods in the TClassOwner
    /// </summary>
    public abstract class MethodContext<TClassOwner> where TClassOwner : class
    {
        /// <summary>
        /// The class instance which will have it's methods invoked via linq
        /// </summary>
        protected TClassOwner OwnerClass { get; private set; }

        /// <summary>
        /// Initialise a new method context
        /// </summary>
        /// <param name="owner">The class instance which will have it's methods invoked via linq</param>
        public MethodContext(TClassOwner owner)
        {
            OwnerClass = owner;
        }
     }
}
