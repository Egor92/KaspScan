using System;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Input;
using Egor92.CollectionExtensions;
using KaspScan.Dependencies;
using KaspScan.Enums;
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
                InitializeProgressProperty(),
                InitializeIsReportsButtonVisibleProperty(),
                InitializeBoldMessageProperty(),
                InitializeThinMessageProperty(),
                InitializeScanningResultProperty(),
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
            return _lastScanningTime = _scanningManager.LastScanningTimeChanged.Where(_ => IsActive)
                                                       .ToProperty(this, x => x.LastScanningTime, null);
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
            return _progress = _scanningManager.StepPassed.Where(_ => IsActive)
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
            return _boldMessage = _scanningManager.StepPassed.Where(_ => IsActive)
                                                  .Select(GetBoldMessage)
                                                  .ToProperty(this, x => x.BoldMessage, GetBoldMessage());
        }

        private string GetBoldMessage()
        {
            var stepInfo = new ScanningAlgorithmStepInfo(_scanningManager.Status, 0.0, null, 0);
            return GetBoldMessage(stepInfo);
        }

        private string GetBoldMessage(ScanningAlgorithmStepInfo stepInfo)
        {
            switch (_scanningManager.Status)
            {
                case AlgorithmStatus.NotRunned:
                    return "Рекомендуется запустить проверку";
                case AlgorithmStatus.Running:
                case AlgorithmStatus.Paused:
                case AlgorithmStatus.Stopped:
                    return $"В результате проверки найдено {stepInfo.WarningCount} проблем";
                case AlgorithmStatus.Finished:
                    if (stepInfo.WarningCount == 0)
                        return "Проблем не убнаружено. Рекомендуется установить защиту";

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
            Expression<Func<MainViewModel, IObservable<ScanningAlgorithmStepInfo>>> stepPassedChanged =
                x => x._scanningManager.StepPassed;
            Expression<Func<MainViewModel, IObservable<TimeSpan?>>> lastScanningTimeChanged =
                x => x._scanningManager.LastScanningTimeChanged;
            return _thinMessage = this.WhenAnyObservable(stepPassedChanged, lastScanningTimeChanged,
                                                         (x1, x2) => new Tuple<ScanningAlgorithmStepInfo, TimeSpan?>(x1, x2))
                                      .Where(_ => IsActive)
                                      .Select(GetThinMessage)
                                      .ToProperty(this, x => x.ThinMessage, GetThinMessage());
        }

        private string GetThinMessage()
        {
            var algorithmStatus = new ScanningAlgorithmStepInfo(_scanningManager.Status, Progress, null, 0);
            TimeSpan? lastScanningTime = null;
            var tuple = new Tuple<ScanningAlgorithmStepInfo, TimeSpan?>(algorithmStatus, lastScanningTime);
            return GetThinMessage(tuple);
        }

        private static string GetThinMessage(Tuple<ScanningAlgorithmStepInfo, TimeSpan?> tuple)
        {
            var stepInfo = tuple.Item1;
            var timeSpan = tuple.Item2;

            if (stepInfo.Status == AlgorithmStatus.NotRunned || timeSpan == null)
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
            return _isReportsButtonVisible = _scanningManager.StepPassed.Select(_ => GetIsReportsButtonVisible())
                                                             .ToProperty(this, x => x.IsReportsButtonVisible,
                                                                         GetIsReportsButtonVisible());
        }

        private bool GetIsReportsButtonVisible()
        {
            return _scanningManager.Status != AlgorithmStatus.NotRunned;
        }

        #endregion

        #region ScanningResult

        private ObservableAsPropertyHelper<ScanningResult> _scanningResult;

        public ScanningResult ScanningResult
        {
            get { return _scanningResult.Value; }
        }

        private IDisposable InitializeScanningResultProperty()
        {
            return _scanningResult = _scanningManager.StepPassed.Select(GetScanningResult)
                                                     .ToProperty(this, x => x.ScanningResult, GetScanningResult());
        }

        private ScanningResult GetScanningResult()
        {
            var stepInfo = new ScanningAlgorithmStepInfo(_scanningManager.Status, Progress, null, 0);
            return GetScanningResult(stepInfo);
        }

        private ScanningResult GetScanningResult(ScanningAlgorithmStepInfo stepInfo)
        {
            if (stepInfo.Status == AlgorithmStatus.NotRunned)
                return ScanningResult.NotRunned;

            if (stepInfo.Status != AlgorithmStatus.Finished)
                return ScanningResult.Running;

            if (stepInfo.WarningCount == 0)
                return ScanningResult.HasNoWarnings;

            return ScanningResult.HasWarnings;
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
            if (_scanningManager.Status != AlgorithmStatus.Running)
            {
                _scanningManager.Start();
            }
        }

        #endregion

        #endregion

        #endregion
    }
}
