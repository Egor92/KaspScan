using System.Windows.Controls;
using KaspScan.ViewModels;

namespace KaspScan.Views
{
    public partial class ScanningView : UserControl
    {
        public ScanningView(ScanningViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
