using System.Windows.Controls;
using KaspScan.ViewModels;

namespace KaspScan.Views
{
    public partial class BottomMenuView : UserControl
    {
        public BottomMenuView(BottomMenuViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
