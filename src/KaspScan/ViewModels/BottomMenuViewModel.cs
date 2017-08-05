using KaspScan.Dependencies;
using KaspScan.ViewModels.Base;

namespace KaspScan.ViewModels
{
    public class BottomMenuViewModel : ActiveAwareViewModel
    {
        #region Ctor

        public BottomMenuViewModel(ISchedulers schedulers)
            : base(schedulers)
        {
        }

        #endregion
    }
}
