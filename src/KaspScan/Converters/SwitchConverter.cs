using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Markup;
using KaspScan.Converters.Cases;

namespace KaspScan.Converters

{
    [ContentProperty("Cases")]
    public class SwitchConverter : ChainConverter<object, object>
    {
        public List<ICase> Cases { get; private set; }

        public object Default { get; set; }

        public SwitchConverter()
        {
            Cases = new List<ICase>();
        }

        protected override object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            foreach (var @case in Cases)
            {
                if (@case.IsMatched(value))
                    return @case.Value;
            }
            return Default;
        }

        protected override object ConvertBack(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            throw new NotSupportedException();
        }
    }
}