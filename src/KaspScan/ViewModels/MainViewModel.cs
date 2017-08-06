using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Egor92.CollectionExtensions;
using KaspScan.Dependencies;
using KaspScan.Managers;
using KaspScan.Model;
using KaspScan.ViewModels.Base;
using ReactiveUI;

namespace KaspScan.ViewModels
{
    public class MainViewModel : ActiveAwareViewModel
    {
        #region Fields

        private readonly IScanningManager _scanningManager;

        #endregion

        #region Ctor

        public MainViewModel(ISchedulers schedulers, IScanningManager scanningManager)
            : base(schedulers)
        {
            _scanningManager = scanningManager;

            Initialize();
        }

        private void Initialize()
        {
            Disposables.AddRange(new[]
            {
                InitializeLastScanningTimeProperty(),
                InitializeAlgorithmStatusProperty(),
                InitializeProgressProperty(),
            });
        }

        #endregion

        #region Properties

        private IObservable<bool> IsActiveChanged
        {
            get
            {
                return this.ObservableForProperty(x => x.IsActive)
                           .Select(x => x.Value);
            }
        }

        #region LastScanningTime

        private ObservableAsPropertyHelper<TimeSpan?> _lastScanningTime;

        public TimeSpan? LastScanningTime
        {
            get { return _lastScanningTime.Value; }
        }

        private IDisposable InitializeLastScanningTimeProperty()
        {
            Expression<Func<MainViewModel, IObservable<TimeSpan?>>> lastScanningTimeChanged =
                x => x._scanningManager.LastScanningTimeChanged;
            return _lastScanningTime = this.WhenAnyObservable(lastScanningTimeChanged, x => x.IsActiveChanged, (x1, x2) => x1)
                                           .DistinctUntilChanged()
                                           .Where(_ => IsActive)
                                           .ToProperty(this, x => x.LastScanningTime, null);
        }

        #endregion

        #region AlgorithmStatus

        private ObservableAsPropertyHelper<AlgorithmStatus> _algorithmStatus;

        public AlgorithmStatus AlgorithmStatus
        {
            get { return _algorithmStatus.Value; }
        }

        private IDisposable InitializeAlgorithmStatusProperty()
        {
            const AlgorithmStatus initialAlgorithmStatus = AlgorithmStatus.NotStarted;
            Expression<Func<MainViewModel, IObservable<AlgorithmStatus>>> algorithmStatusChanged =
                x => x._scanningManager.StatusChanged;
            return _algorithmStatus = this.WhenAnyObservable(algorithmStatusChanged, x => x.IsActiveChanged, (x1, x2) => x1)
                                          .DistinctUntilChanged()
                                          .Where(_ => IsActive)
                                          .ToProperty(this, x => x.AlgorithmStatus, initialAlgorithmStatus);
        }

        #endregion

        #region Progress

        private ObservableAsPropertyHelper<double> _progress;

        public double Progress
        {
            get { return _progress.Value; }
        }

        private IDisposable InitializeProgressProperty()
        {
            Expression<Func<MainViewModel, IObservable<ScanningAlgorithmStepInfo>>> stepPassed =
                x => x._scanningManager.StepPassed;
            return _progress = this.WhenAnyObservable(stepPassed, x => x.IsActiveChanged, (x1, x2) => x1)
                                   .DistinctUntilChanged()
                                   .Where(_ => IsActive)
                                   .Select(y => y.Progress)
                                   .ToProperty(this, x => x.Progress);
        }

        #endregion

        #region Commands

        #region StartScanningCommand

        private ICommand _startScanningCommand;

        public ICommand StartScanningCommand
        {
            get { return _startScanningCommand ?? (_startScanningCommand = GetStartScanningCommand()); }
        }

        private ICommand GetStartScanningCommand()
        {
            return ReactiveCommand.Create(StartScanning)
                                  .DisposeWith(Disposables);
        }

        private void StartScanning()
        {
            if (_scanningManager.Status != AlgorithmStatus.Started)
            {
                _scanningManager.Start();
            }
        }

        #endregion

        #endregion

        #endregion

        #region Overriden members

        protected override IEnumerable<IDisposable> GetSubscriptionsOnActivation()
        {
            yield return _scanningManager.Finished.Subscribe(OnScanningFinished);
        }

        #endregion

        private void OnScanningFinished(Unit unit)
        {
        }
    }
}
