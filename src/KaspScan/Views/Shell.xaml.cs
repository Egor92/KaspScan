using System.Windows;
using KaspScan.ViewModels;

namespace KaspScan.Views
{
    public partial class Shell : Window
    {
        public Shell(ShellViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
