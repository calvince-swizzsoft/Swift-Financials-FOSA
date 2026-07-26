using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Application.Seedwork
{
    [DataContract]
    public abstract class BindingModelBase<TBindingModel> : INotifyPropertyChanged, INotifyDataErrorInfo, IComparable<TBindingModel>
        where TBindingModel : BindingModelBase<TBindingModel>
    {
        private readonly List<PropertyValidation<TBindingModel>> _validations = new List<PropertyValidation<TBindingModel>>();
        private Dictionary<string, List<string>> _errorMessages = new Dictionary<string, List<string>>();

        public BindingModelBase()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName != "HasErrors")
                    ValidateProperty(e.PropertyName);
            };
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region INotifyDataErrorInfo

        public IEnumerable GetErrors(string propertyName)
        {
            if (_errorMessages.ContainsKey(propertyName))
                return _errorMessages[propertyName];

            return new string[0];
        }

        [Description("Has Errors?")]
        [ReadOnly(true)]
        public bool HasErrors
        {
            get { return _errorMessages.Count > 0; }
        }

        [Description("Error Messages")]
        [ReadOnly(true)]
        public List<string> ErrorMessages
        {
            get
            {
                var temp = new List<string>();

                foreach (var kvp in _errorMessages)
                {
                    foreach (var item in kvp.Value)
                    {
                        temp.Add(string.Format("{0}", item));
                    }
                }

                return temp;
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged = delegate { };

        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        #endregion

        protected void OnPropertyChanged(Expression<Func<object>> expression)
        {
            OnPropertyChanged(GetPropertyName(expression));
        }

        protected PropertyValidation<TBindingModel> AddValidationFor(Expression<Func<object>> expression)
        {
            return AddValidationFor(GetPropertyName(expression));
        }

        protected PropertyValidation<TBindingModel> AddValidationFor(string propertyName)
        {
            var validation = new PropertyValidation<TBindingModel>(propertyName);
            _validations.Add(validation);

            return validation;
        }

        protected void AddAllAttributeValidators()
        {
            PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                Attribute[] custom = Attribute.GetCustomAttributes(propertyInfo, typeof(ValidationAttribute), true);
                foreach (var attribute in custom)
                {
                    var property = propertyInfo;
                    var validationAttribute = attribute as ValidationAttribute;

                    if (validationAttribute == null)
                        throw new NotSupportedException("validationAttribute variable should be inherited from ValidationAttribute type");

                    string name = property.Name;

                    var displayAttribute = Attribute.GetCustomAttributes(propertyInfo, typeof(DisplayAttribute)).FirstOrDefault() as DisplayAttribute;
                    if (displayAttribute != null)
                    {
                        name = displayAttribute.GetName();
                    }

                    var message = validationAttribute.FormatErrorMessage(name);

                    AddValidationFor(propertyInfo.Name)
                        .When(x =>
                        {
                            var value = property.GetGetMethod().Invoke(this, new object[] { });
                            var result = validationAttribute.GetValidationResult(value, new ValidationContext(this, null, null) { MemberName = property.Name });
                            return result != ValidationResult.Success;
                        })
                        .Show(message);

                }
            }
        }

        public void ValidateAll()
        {
            var propertyNamesWithValidationErrors = _errorMessages.Keys;

            _errorMessages = new Dictionary<string, List<string>>();

            _validations.ForEach(PerformValidation);

            var propertyNamesThatMightHaveChangedValidation =
                _errorMessages.Keys.Union(propertyNamesWithValidationErrors).ToList();

            propertyNamesThatMightHaveChangedValidation.ForEach(OnErrorsChanged);

            OnPropertyChanged(() => HasErrors);
        }

        public void ValidateProperty(Expression<Func<object>> expression)
        {
            ValidateProperty(GetPropertyName(expression));
        }

        void ValidateProperty(string propertyName)
        {
            _errorMessages.Remove(propertyName);

            _validations.Where(v => v.PropertyName == propertyName).ToList().ForEach(PerformValidation);

            OnErrorsChanged(propertyName);

            OnPropertyChanged(() => HasErrors);
        }

        void PerformValidation(PropertyValidation<TBindingModel> validation)
        {
            if (validation.IsInvalid((TBindingModel)this))
            {
                AddErrorMessageForProperty(validation.PropertyName, validation.GetErrorMessage());
            }
        }

        void AddErrorMessageForProperty(string propertyName, string errorMessage)
        {
            if (_errorMessages.ContainsKey(propertyName))
            {
                _errorMessages[propertyName].Add(errorMessage);
            }
            else
            {
                _errorMessages.Add(propertyName, new List<string> { errorMessage });
            }
        }

        static string GetPropertyName(Expression<Func<object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            MemberExpression memberExpression;

            if (expression.Body is UnaryExpression)
                memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            else
                memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null)
                throw new ArgumentException("The expression is not a member access expression", "expression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException("The member access expression does not access a property", "expression");

            var getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException("The referenced property is a static property", "expression");

            return memberExpression.Member.Name;
        }

        public int CompareTo(TBindingModel other)
        {
            return this.CompareTo(other);
        }
    }
}
