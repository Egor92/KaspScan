using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Windows.Input;
using KaspScan.Enums;
using KaspScan.Extensions;
using KaspScan.Helpers;
using KaspScan.Managers;
using KaspScan.Model;
using KaspScan.Reacitve;
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
        }

        #endregion

        #region Properties

        #region LastScanningTime

        private ObservableAsPropertyHelper<TimeSpan?> _lastScanningTime;

        public TimeSpan? LastScanningTime
        {
            get { return _lastScanningTime.GetValueOrDefault(); }
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
            get { return _progress.GetValueOrDefault(); }
        }

        private IDisposable InitializeProgressProperty()
        {
            return _progress = _scanningManager.FileScanned.Where(_ => IsActive)
                                               .Select(y => y.Progress)
                                               .ToProperty(this, x => x.Progress);
        }

        #endregion

        #region BoldMessage

        private string _boldMessage;

        public string BoldMessage
        {
            get { return _boldMessage; }
            private set { this.RaiseAndSetIfChanged(ref _boldMessage, value); }
        }

        private IDisposable SubscribeToBoldMessageUpdating()
        {
            return _scanningManager.FileScanned.Select(GetBoldMessage)
                                   .Subscribe(UpdateBoldMessage);
        }

        private string GetBoldMessage(FileScannedInfo fileScannedInfo)
        {
            switch (_scanningManager.Status)
            {
                case AlgorithmStatus.NotRunned:
                    return "Рекомендуется запустить проверку";
                case AlgorithmStatus.Running:
                case AlgorithmStatus.Paused:
                case AlgorithmStatus.Stopped:
                    var problemWord = NumeralsHelpers.GetFeminineWordInNominativeCase("проблем", fileScannedInfo.WarningCount);
                    return $"В результате проверки найдено {fileScannedInfo.WarningCount} {problemWord}";
                case AlgorithmStatus.Finished:
                    if (fileScannedInfo.WarningCount == 0)
                        return "Проблем не убнаружено. Рекомендуется установить защиту";

                    problemWord = NumeralsHelpers.GetFeminineWordInNominativeCase("проблем", fileScannedInfo.WarningCount);
                    return $"В результате проверки найдено {fileScannedInfo.WarningCount} {problemWord}";

                default:
                    throw new Exception($"Unhandled case with {_scanningManager.Status}");
            }
        }

        private void UpdateBoldMessage(string value)
        {
            BoldMessage = value;
        }

        #endregion

        #region ThinMessage

        private string _thinMessage;

        public string ThinMessage
        {
            get { return _thinMessage; }
            private set { this.RaiseAndSetIfChanged(ref _thinMessage, value); }
        }

        private IDisposable SubscribeToThinMessageUpdating()
        {
            var fileScanned = _scanningManager.FileScanned;
            var lastScanningTimeChanged = _scanningManager.LastScanningTimeChanged;

            return Observable.CombineLatest(fileScanned, lastScanningTimeChanged,
                                            (x1, x2) => new Tuple<FileScannedInfo, TimeSpan?>(x1, x2))
                             .Select(GetThinMessage)
                             .Subscribe(UpdateThinMessage);
        }

        private static string GetThinMessage(Tuple<FileScannedInfo, TimeSpan?> tuple)
        {
            var fileScannedInfo = tuple.Item1;
            var timeSpan = tuple.Item2;

            if (fileScannedInfo.Status == AlgorithmStatus.NotRunned)
                return "Проверка ещё не запускалась";

            if (fileScannedInfo.Status == AlgorithmStatus.Running ||
                fileScannedInfo.Status == AlgorithmStatus.Paused ||
                fileScannedInfo.Status == AlgorithmStatus.Stopped)
                return null;

            if (timeSpan == null)
                return null;

            var lastScanningTime = timeSpan.Value;
            var hours = lastScanningTime.Hours;
            var hoursWord = NumeralsHelpers.GetMasculineWordInDativeCase("час", hours);
            var minutes = lastScanningTime.Minutes;
            var minutesWord = NumeralsHelpers.GetFeminineWordInDativeCase("минут", minutes);
            var seconds = lastScanningTime.Seconds;
            var secondsWord = NumeralsHelpers.GetFeminineWordInDativeCase("секунд", seconds);
            return $"Последняя проверка компьютера: {hours} {hoursWord} {minutes} {minutesWord} {seconds} {secondsWord} назад";
        }

        private void UpdateThinMessage(string value)
        {
            ThinMessage = value;
        }

        #endregion

        #region IsReportsButtonVisible

        private ObservableAsPropertyHelper<bool> _isReportsButtonVisible;

        public bool IsReportsButtonVisible
        {
            get { return _isReportsButtonVisible.GetValueOrDefault(); }
        }

        private IDisposable InitializeIsReportsButtonVisibleProperty()
        {
            return _isReportsButtonVisible = _scanningManager.FileScanned.Select(GetIsReportsButtonVisible)
                                                             .ToProperty(this, x => x.IsReportsButtonVisible);
        }

        private static bool GetIsReportsButtonVisible(FileScannedInfo fileScannedInfo)
        {
            return fileScannedInfo.Status != AlgorithmStatus.NotRunned;
        }

        #endregion

        #region ScanningResult

        private ObservableAsPropertyHelper<ScanningResult> _scanningResult;

        public ScanningResult ScanningResult
        {
            get { return _scanningResult.GetValueOrDefault(); }
        }

        private IDisposable InitializeScanningResultProperty()
        {
            return _scanningResult = _scanningManager.FileScanned.Select(GetScanningResult)
                                                     .ToProperty(this, x => x.ScanningResult);
        }

        private static ScanningResult GetScanningResult(FileScannedInfo fileScannedInfo)
        {
            if (fileScannedInfo.Status == AlgorithmStatus.NotRunned)
                return ScanningResult.NotRunned;

            if (fileScannedInfo.Status != AlgorithmStatus.Finished)
                return ScanningResult.Running;

            if (fileScannedInfo.WarningCount == 0)
                return ScanningResult.HasNoWarnings;

            return ScanningResult.HasWarnings;
        }

        #endregion

        #region IsScanningProgressVisible

        private ObservableAsPropertyHelper<bool> _isScanningProgressVisible;

        public bool IsScanningProgressVisible
        {
            get { return _isScanningProgressVisible.GetValueOrDefault(); }
        }

        private IDisposable InitializeIsScanningProgressVisibleProperty()
        {
            return _isScanningProgressVisible = _scanningManager.FileScanned.Select(GetIsScanningProgressVisible)
                                                                .ToProperty(this, x => x.IsScanningProgressVisible);
        }

        private static bool GetIsScanningProgressVisible(FileScannedInfo fileScannedInfo)
        {
            return fileScannedInfo.Status != AlgorithmStatus.NotRunned;
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
            var command = ReactiveCommand.Create(StartScanning);
            AddDisposable(command);
            return command;
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

        #region Overridden members

        protected override IEnumerable<IDisposable> GetSubscriptionsOnActivation()
        {
            yield return InitializeLastScanningTimeProperty();
            yield return InitializeProgressProperty();
            yield return InitializeIsReportsButtonVisibleProperty();
            yield return SubscribeToBoldMessageUpdating();
            yield return SubscribeToThinMessageUpdating();
            yield return InitializeScanningResultProperty();
            yield return InitializeIsScanningProgressVisibleProperty();
        }

        #endregion
    }
}
