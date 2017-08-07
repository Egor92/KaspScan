using KaspScan.Reacitve;
using KaspScan.ViewModels.Base;

namespace KaspScan.ViewModels
{
    public class ScanningViewModel : ActiveAwareViewModel
    {
        #region Ctor

        public ScanningViewModel(ISchedulers schedulers)
            : base(schedulers)
        {
        }

        #endregion
    }
}
