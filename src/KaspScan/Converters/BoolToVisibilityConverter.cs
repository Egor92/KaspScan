using System.Windows;

namespace KaspScan.Converters
{
    public class BoolToVisibilityConverter : BoolConverterBase<Visibility>
    {
        protected override Visibility GetDefaultTrueValue()
        {
            return Visibility.Visible;
        }

        protected override Visibility GetDefaultFalseValue()
        {
            return Visibility.Collapsed;
        }
    }
}
