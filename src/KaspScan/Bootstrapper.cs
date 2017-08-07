using System;
using System.Windows;
using KaspScan.Constants;
using KaspScan.Managers;
using KaspScan.Reacitve;
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

            AttachAppBehaviors();
            RegisterViews();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            Container.RegisterInstance<ISchedulers>(new Schedulers(Application.Current.Dispatcher));
            Container.RegisterType<IScanningManager, ScanningManager>(new ContainerControlledLifetimeManager());

            Container.RegisterTypeForNavigation<MainView>(NavigationViewNames.Main);
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

        private void AttachAppBehaviors()
        {
            var appDomainExceptionHandleManager = new AppDomainExceptionHandleManager(AppDomain.CurrentDomain);
            appDomainExceptionHandleManager.Attach();
        }

        private void RegisterViews()
        {
            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RequestNavigate(RegionNames.MainWorkspaceRegion, NavigationViewNames.Main);
            regionManager.RegisterViewWithRegion(RegionNames.BottomMenuRegion, typeof(BottomMenuView));
        }
    }
}
