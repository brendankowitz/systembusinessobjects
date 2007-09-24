using System;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Validates false if the target property is more than the speficified length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationLengthAttribute : Attribute
    {
        private int _minLength;
        internal bool _minLengthSpecified;

        private int _maxLength;
        internal bool _maxLengthSpecified;

        public int MinLength
        {
            get { return _minLength; }
            set { _minLength = value;
                _minLengthSpecified = true; }
        }

        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                _maxLength = value;
                _maxLengthSpecified = true;
            }
        }
    }

    /// <summary>
    /// Validates false if the target property is empty or null
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationNotEmptyAttribute : Attribute
    {

    }
}
