using KaspScan.Dependencies;
using KaspScan.ViewModels.Base;

namespace KaspScan.ViewModels
{
    public class ShellViewModel : ActiveAwareViewModel
    {
        #region Ctor

        public ShellViewModel(ISchedulers schedulers)
            : base(schedulers)
        {
        }

        #endregion
    }
}
