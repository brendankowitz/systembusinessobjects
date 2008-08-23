using System;
using System.Collections.Generic;
using System.BusinessObjects.Data;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Abstract base attribute giving the definition for providing Validation rules
    /// </summary>
    public abstract class ValidationBaseAttribute : Attribute
    {
        public abstract IList<ValidationRule> GetValidationRules(DataObject current, string property);
    }

    /// <summary>
    /// Validates false if the target property is more than the speficified length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationLengthAttribute : ValidationBaseAttribute
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

        public override IList<ValidationRule> GetValidationRules(DataObject current, string property)
        {
            IList<ValidationRule> rules = new List<ValidationRule>();
            if (_maxLengthSpecified)
            {
                ValidationRule rule =
                    new ValidationRule(
                        GeneralAssertionTemplate.LengthLess(current, property, MaxLength));
                rules.Add(rule);
            }
            if (_minLengthSpecified)
            {
                ValidationRule rule =
                    new ValidationRule(
                        GeneralAssertionTemplate.LengthGreater(current, property, MinLength));
                rules.Add(rule);
            }
            return rules;
        }
    }

    /// <summary>
    /// Validates false if the target property is empty or null
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationNotEmptyAttribute : ValidationBaseAttribute
    {
        public override IList<ValidationRule> GetValidationRules(DataObject current, string property)
        {
            IList<ValidationRule> rules = new List<ValidationRule>();
            ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.IsNotEmpty(current, property));
            rules.Add(rule);
            return rules;
        }
    }

    /// <summary>
    /// Validates false if the target property is empty or null by checking the DataObject.IsNull() function
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationIsNotNullAttribute : ValidationBaseAttribute
    {
        public override IList<ValidationRule> GetValidationRules(DataObject current, string property)
        {
            IList<ValidationRule> rules = new List<ValidationRule>();
            ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.IsBusinessObjectNotNull(current, property));
            rules.Add(rule);
            return rules;
        }
    }
}
