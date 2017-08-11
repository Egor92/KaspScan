using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KaspScan.Model;
using KaspScan.ViewModels.Base;

namespace KaspScan.Managers
{
    public interface IScanningManager
    {
        AlgorithmStatus Status { get; }

        IObservable<FileScannedInfo> FileScanned { get; }

        IObservable<TimeSpan?> LastScanningTimeChanged { get; }

        void Start();

        void Pause();

        void Stop();
    }

    public class ScanningManager : DisposableViewModel, IScanningManager
    {
        #region Fields

        private readonly ScanningAlgorithm _scanningAlgorithm = new ScanningAlgorithm();
        private DateTime? _lastScanningFinishTime;

        #endregion

        #region Ctor

        public ScanningManager()
        {
            AddDisposables(new[]
            {
                _scanningAlgorithm,
                _scanningAlgorithm.FileScanned.Subscribe(OnFileScanned),
                Observable.Interval(TimeSpan.FromSeconds(1))
                          .Subscribe(OnNextTime),
            });
        }

        #endregion

        #region Properties

        #region Status

        public AlgorithmStatus Status
        {
            get { return _scanningAlgorithm.Status; }
        }

        #endregion

        #endregion

        #region Events

        #region FileScanned

        public IObservable<FileScannedInfo> FileScanned
        {
            get { return _scanningAlgorithm.FileScanned; }
        }

        #endregion

        #region LastScanningTimeChanged

        private readonly BehaviorSubject<TimeSpan?> _lastScanningTimeChanged = new BehaviorSubject<TimeSpan?>(null);

        public IObservable<TimeSpan?> LastScanningTimeChanged
        {
            get { return _lastScanningTimeChanged.DistinctUntilChanged(); }
        }

        #endregion

        #endregion

        #region Overridden members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _scanningAlgorithm.Dispose();
            }
        }

        #endregion

        #region Public methods

        public void Start()
        {
            _scanningAlgorithm.Start();
        }

        public void Pause()
        {
            _scanningAlgorithm.Pause();
        }

        public void Stop()
        {
            _scanningAlgorithm.Stop();
        }

        #endregion

        private void OnNextTime(long arg)
        {
            if (_lastScanningFinishTime == null)
            {
                _lastScanningTimeChanged.OnNext(null);
            }
            else
            {
                var lastScanningTimeHasPassed = DateTime.Now - _lastScanningFinishTime;
                _lastScanningTimeChanged.OnNext(lastScanningTimeHasPassed);
            }
        }

        private void OnFileScanned(FileScannedInfo fileScannedInfo)
        {
            if (fileScannedInfo.Status == AlgorithmStatus.Finished)
            {
                _lastScanningFinishTime = DateTime.Now;
            }
            else
            {
                _lastScanningFinishTime = null;
            }
        }
    }
}
