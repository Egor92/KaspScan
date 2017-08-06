using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Egor92.CollectionExtensions;
using KaspScan.Model;
using KaspScan.ViewModels.Base;

namespace KaspScan.Managers
{
    public interface IScanningManager
    {
        AlgorithmStatus Status { get; }

        IObservable<ScanningAlgorithmStepInfo> StepPassed { get; }

        IObservable<AlgorithmStatus> StatusChanged { get; }

        IObservable<Unit> Finished { get; }

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
            Disposables.AddRange(new[]
            {
                _scanningAlgorithm,
                _scanningAlgorithm.Finished.Subscribe(OnScanningFinished),
                Observable.Interval(TimeSpan.FromSeconds(20))
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

        #region StepPassed

        public IObservable<ScanningAlgorithmStepInfo> StepPassed
        {
            get { return _scanningAlgorithm.StepPassed; }
        }

        #endregion

        #region StatusChanged

        public IObservable<AlgorithmStatus> StatusChanged
        {
            get { return _scanningAlgorithm.StatusChanged; }
        }

        #endregion

        #region Finished

        public IObservable<Unit> Finished
        {
            get { return _scanningAlgorithm.Finished; }
        }

        #endregion

        #region LastScanningTimeChanged

        private readonly Subject<TimeSpan?> _lastScanningTimeChanged = new Subject<TimeSpan?>();

        public IObservable<TimeSpan?> LastScanningTimeChanged
        {
            get { return _lastScanningTimeChanged.AsObservable(); }
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
                return;

            var lastScanningTimeHasPassed = DateTime.Now - _lastScanningFinishTime;
            _lastScanningTimeChanged.OnNext(lastScanningTimeHasPassed);
        }

        private void OnScanningFinished(Unit unit)
        {
            _lastScanningFinishTime = DateTime.Now;
        }
    }
}
