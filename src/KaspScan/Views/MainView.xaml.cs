using System.Windows.Controls;
using KaspScan.ViewModels;

namespace KaspScan.Views
{
    public partial class MainView : UserControl
    {
        public MainView(MainViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
