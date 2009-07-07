using System;
using System.Collections.Generic;
using System.BusinessObjects.Data;
using System.Text.RegularExpressions;

namespace System.BusinessObjects.Validation
{
    /// <summary>
    /// Abstract base attribute giving the definition for providing Validation rules
    /// </summary>
    [Serializable]
    public abstract class ValidationBaseAttribute : Attribute
    {
        public abstract IList<ValidationRule> GetValidationRules(DataObject current, string property);
    }

    /// <summary>
    /// Validates false if the target property is more than the speficified length
    /// </summary>
    [Serializable]
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
    [Serializable]
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
    [Serializable]
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

    /// <summary>
    /// Validates false if the target property fails the regex pattern
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class ValidationRegexAttribute : ValidationBaseAttribute
    {
        private string _ErrorMessage;
        public string ErrorMessage
        {
            get
            {
                return _ErrorMessage;
            }
            set
            {
                _ErrorMessage = value;
            }
        }

        string regexExpression;
        private RegexOptions _RegexOptions = RegexOptions.None;
        public RegexOptions RegexOptions
        {
            get
            {
                return _RegexOptions;
            }
            set
            {
                _RegexOptions = value;
            }
        }

        public ValidationRegexAttribute(string expression)
        {
            regexExpression = expression;
        }

        public override IList<ValidationRule> GetValidationRules(DataObject current, string property)
        {
            if(string.IsNullOrEmpty(regexExpression))
            {
                throw new ApplicationException(string.Format("A regex validation expression must be supplied on property {0}", property));
            }
            if(string.IsNullOrEmpty(ErrorMessage))
            {
                ErrorMessage = "{0} failed regex validation";
            }

            Regex r = new Regex(regexExpression, RegexOptions);

            IList<ValidationRule> rules = new List<ValidationRule>();
            ValidationRule rule = new ValidationRule(GeneralAssertionTemplate.Regex(current, property, r, ErrorMessage));
            rules.Add(rule);
            return rules;
        }
    }
}
