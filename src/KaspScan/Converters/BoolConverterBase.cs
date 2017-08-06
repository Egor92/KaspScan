using System;
using System.Globalization;

namespace KaspScan.Converters
{
    public abstract class BoolConverterBase<T> : ChainConverter<bool, T>
    {
        private bool _isTrueValueInitialized;
        private bool _isFalseValueInitialized;
        private T _trueValue;
        private T _falseValue;

        public T TrueValue
        {
            get
            {
                if (!_isTrueValueInitialized)
                {
                    _trueValue = GetDefaultTrueValue();
                    _isTrueValueInitialized = true;
                }
                return _trueValue;

            }
            set
            {
                _trueValue = value;
                _isTrueValueInitialized = true;
            }
        }

        public T FalseValue
        {
            get
            {
                if (!_isFalseValueInitialized)
                {
                    _falseValue = GetDefaultFalseValue();
                    _isFalseValueInitialized = true;
                }
                return _falseValue;

            }
            set
            {
                _falseValue = value;
                _isFalseValueInitialized = true;
            }
        }

        protected virtual T GetDefaultTrueValue()
        {
            return default(T);
        }

        protected virtual T GetDefaultFalseValue()
        {
            return default(T);
        }

        protected sealed override T Convert(bool value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return ConvertFromBool(value);
        }

        protected sealed override bool ConvertBack(T value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            return ConvertToBool(value);
        }

        private T ConvertFromBool(bool value)
        {
            return value
                ? TrueValue
                : FalseValue;
        }

        private bool ConvertToBool(T value)
        {
            return value.Equals(TrueValue);
        }
    }
}
