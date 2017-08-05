using System.Windows;
using KaspScan.Constants;
using KaspScan.Dependencies;
using KaspScan.Views;
using Microsoft.Practices.Unity;
using Prism.Regions;
using Prism.Unity;

namespace KaspScan
{
    internal class Bootstrapper : UnityBootstrapper
    {
        public override void Run(bool runWithDefaultConfiguration)
        {
            base.Run(runWithDefaultConfiguration);

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.MainWorkspaceRegion, NavigationViewNames.UserChoice);
            regionManager.RegisterViewWithRegion(RegionNames.BottomMenuRegion, typeof(BottomMenuView));
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance<ISchedulers>(new Schedulers(Application.Current.Dispatcher));

            Container.RegisterTypeForNavigation<UserChoiceView>(NavigationViewNames.UserChoice);
            Container.RegisterTypeForNavigation<ScanningView>(NavigationViewNames.Scanning);
        }

        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow = (Window)Shell;
            Application.Current.MainWindow.Show();
        }
    }
}
