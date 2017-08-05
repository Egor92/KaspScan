using System.Windows.Controls;
using KaspScan.ViewModels;

namespace KaspScan.Views
{
    public partial class UserChoiceView : UserControl
    {
        public UserChoiceView(UserChoiceViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
