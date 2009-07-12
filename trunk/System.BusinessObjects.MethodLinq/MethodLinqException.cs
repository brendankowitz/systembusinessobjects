using System;

namespace System.BusinessObjects.MethodLinq
{
    public class MethodLinqException : Exception
    {
        public MethodLinqException(string message) : base(message) { }
        public MethodLinqException(string message, Exception inner) : base(message, inner) { }
    }
}
