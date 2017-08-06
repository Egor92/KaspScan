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
        private readonly IObservable<bool> _isActiveChanged;

        #endregion

        #region Ctor

        public MainViewModel(ISchedulers schedulers, IScanningManager scanningManager)
            : base(schedulers)
        {
            _scanningManager = scanningManager;

            _isActiveChanged = this.ObservableForProperty(x => x.IsActive)
                                   .Select(x => x.Value);
            Initialize();
        }

        private void Initialize()
        {
            Disposables.AddRange(new[]
            {
                InitializeLastScanningTimeProperty(),
                InitializeAlgorithmStatusProperty(),
                InitializeProgressProperty(),
                InitializeIsReportsButtonVisibleProperty(),
                InitializeBoldMessageProperty(),
                InitializeThinMessageProperty(),
            });
        }

        #endregion

        #region Properties

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
            return _lastScanningTime = this.WhenAnyObservable(lastScanningTimeChanged, x => x._isActiveChanged, (x1, x2) => x1)
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
            return _algorithmStatus = this.WhenAnyObservable(algorithmStatusChanged, x => x._isActiveChanged, (x1, x2) => x1)
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
            Expression<Func<MainViewModel, IObservable<ScanningAlgorithmStepInfo>>> stepPassed = x => x._scanningManager.StepPassed;
            return _progress = this.WhenAnyObservable(stepPassed, x => x._isActiveChanged, (x1, x2) => x1)
                                   .Where(_ => IsActive)
                                   .Select(y => y.Progress)
                                   .ToProperty(this, x => x.Progress);
        }

        #endregion

        #region BoldMessage

        private ObservableAsPropertyHelper<string> _boldMessage;

        public string BoldMessage
        {
            get { return _boldMessage.Value; }
        }

        private IDisposable InitializeBoldMessageProperty()
        {
            Expression<Func<MainViewModel, IObservable<AlgorithmStatus>>> algorithmStatusChanged =
                x => x._scanningManager.StatusChanged;
            Expression<Func<MainViewModel, IObservable<ScanningAlgorithmStepInfo>>> stepPassed = x => x._scanningManager.StepPassed;
            return _boldMessage = this
                .WhenAnyObservable(algorithmStatusChanged, stepPassed, x => x._isActiveChanged, (x1, x2, x3) => x2)
                .Where(_ => IsActive)
                .Select(GetBoldMessage)
                .ToProperty(this, x => x.BoldMessage, GetBoldMessage());
        }

        private string GetBoldMessage()
        {
            var stepInfo = new ScanningAlgorithmStepInfo(0.0, null, 0);
            return GetBoldMessage(stepInfo);
        }

        private string GetBoldMessage(ScanningAlgorithmStepInfo stepInfo)
        {
            switch (_scanningManager.Status)
            {
                case AlgorithmStatus.NotStarted:
                    return "Рекомендуется запустить проверку";
                case AlgorithmStatus.Started:
                case AlgorithmStatus.Paused:
                case AlgorithmStatus.Stopped:
                case AlgorithmStatus.Finished:
                    return $"В результате проверки найдено {stepInfo.WarningCount} проблем";
                default:
                    throw new Exception($"Unhandled case with {_scanningManager.Status}");
            }
        }

        #endregion

        #region ThinMessage

        private ObservableAsPropertyHelper<string> _thinMessage;

        public string ThinMessage
        {
            get { return _thinMessage.Value; }
        }

        private IDisposable InitializeThinMessageProperty()
        {
            Expression<Func<MainViewModel, IObservable<AlgorithmStatus>>> algorithmStatusChanged =
                x => x._scanningManager.StatusChanged;
            Expression<Func<MainViewModel, IObservable<TimeSpan?>>> lastScanningTimeChanged =
                x => x._scanningManager.LastScanningTimeChanged;
            return _thinMessage = this.WhenAnyObservable(algorithmStatusChanged, lastScanningTimeChanged, x => x._isActiveChanged,
                                                         (x1, x2, x3) => new Tuple<AlgorithmStatus, TimeSpan?>(x1, x2))
                                      .Where(_ => IsActive)
                                      .Select(GetThinMessage)
                                      .ToProperty(this, x => x.ThinMessage, GetThinMessage());
        }

        private static string GetThinMessage()
        {
            var algorithmStatus = AlgorithmStatus.NotStarted;
            TimeSpan? lastScanningTime = null;
            var tuple = new Tuple<AlgorithmStatus, TimeSpan?>(algorithmStatus, lastScanningTime);
            return GetThinMessage(tuple);
        }

        private static string GetThinMessage(Tuple<AlgorithmStatus, TimeSpan?> tuple)
        {
            var algorithmStatus = tuple.Item1;
            var timeSpan = tuple.Item2;

            if (algorithmStatus == AlgorithmStatus.NotStarted || timeSpan == null)
                return "Проверка ещё не запускалась";

            var lastScanningTime = timeSpan.Value;
            var hours = lastScanningTime.Hours;
            var minutes = lastScanningTime.Minutes;
            var seconds = lastScanningTime.Seconds;
            return $"Последняя проверка компьютера: {hours} часов {minutes} минут {seconds} секунд назад";
        }

        #endregion

        #region IsReportsButtonVisible

        private ObservableAsPropertyHelper<bool> _isReportsButtonVisible;

        public bool IsReportsButtonVisible
        {
            get { return _isReportsButtonVisible.Value; }
        }

        private IDisposable InitializeIsReportsButtonVisibleProperty()
        {
            return _isReportsButtonVisible = this.WhenAnyValue(x => x.AlgorithmStatus)
                                                 .Select(_ => GetIsReportsButtonVisible())
                                                 .ToProperty(this, x => x.IsReportsButtonVisible, GetIsReportsButtonVisible());
        }

        private bool GetIsReportsButtonVisible()
        {
            return AlgorithmStatus != AlgorithmStatus.NotStarted;
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
