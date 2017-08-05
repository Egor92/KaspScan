using KaspScan.Dependencies;
using KaspScan.ViewModels.Base;

namespace KaspScan.ViewModels
{
    public class UserChoiceViewModel : ActiveAwareViewModel
    {
        #region Ctor

        public UserChoiceViewModel(ISchedulers schedulers)
            : base(schedulers)
        {
        }

        #endregion
    }
}
