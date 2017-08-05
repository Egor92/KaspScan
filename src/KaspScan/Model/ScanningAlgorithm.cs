using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using KaspScan.Helpers;

namespace KaspScan.Model
{
    public sealed class ScanningAlgorithm : IDisposable
    {
        #region Fields

        private readonly object _syncRoot = new object();
        private IDisposable _intervalSubscription;
        private readonly TimeSpan _algorithmStepInterval = TimeSpan.FromMilliseconds(100);
        private const long MaxAlgorithmStep = 100;
        private AlgorithmStatus _status = AlgorithmStatus.NotStarted;
        private long _currentAlgorithmStep;
        private bool _generateWarnings;
        private int _warningCount;

        #endregion

        #region Events

        #region StepPassed

        private readonly Subject<ScanningAlgorithmStepInfo> _stepPassed = new Subject<ScanningAlgorithmStepInfo>();

        public IObservable<ScanningAlgorithmStepInfo> StepPassed
        {
            get { return _stepPassed.AsObservable(); }
        }

        #endregion

        #region StatusChanged

        private readonly Subject<AlgorithmStatus> _statusChanged = new Subject<AlgorithmStatus>();

        public IObservable<AlgorithmStatus> StatusChanged
        {
            get { return _statusChanged.AsObservable(); }
        }

        #endregion

        #region Finished

        private readonly Subject<Unit> _finished = new Subject<Unit>();

        public IObservable<Unit> Finished
        {
            get { return _finished.AsObservable(); }
        }

        #endregion

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _intervalSubscription?.Dispose();
            _stepPassed?.Dispose();
            _statusChanged?.Dispose();
            _finished?.Dispose();
        }

        #endregion

        #region Public methods

        public void Start()
        {
            lock (_syncRoot)
            {
                if (_status == AlgorithmStatus.Started)
                    return;

                _status = AlgorithmStatus.Started;
                _currentAlgorithmStep = 0;
                _warningCount = 0;
                _generateWarnings = RandomHelper.GetBool();
                _intervalSubscription = Observable.Interval(_algorithmStepInterval)
                                                  .Subscribe(OnNextAlgorithmStep);
                _statusChanged.OnNext(_status);
            }
        }

        public void Pause()
        {
            lock (_syncRoot)
            {
                if (_status != AlgorithmStatus.Started)
                    return;

                _status = AlgorithmStatus.Paused;
                _intervalSubscription.Dispose();
                _statusChanged.OnNext(_status);
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (_status == AlgorithmStatus.Started || _status == AlgorithmStatus.Paused)
                    return;

                _status = AlgorithmStatus.Stopped;
                _intervalSubscription.Dispose();
                _statusChanged.OnNext(_status);
            }
        }

        #endregion

        private void OnNextAlgorithmStep(long l)
        {
            _currentAlgorithmStep++;
            if (_currentAlgorithmStep > MaxAlgorithmStep)
            {
                _statusChanged.OnNext(AlgorithmStatus.Finished);
                _stepPassed.OnCompleted();
                _finished.OnNext(Unit.Default);
                _finished.OnCompleted();
                return;
            }

            var progress = _currentAlgorithmStep / MaxAlgorithmStep;
            var actualScanningFileName = RandomStringHelper.GetWord(5, 15);
            if (_generateWarnings && RandomHelper.GetBool(0.1))
            {
                _warningCount++;
            }
            _stepPassed.OnNext(new ScanningAlgorithmStepInfo(progress, actualScanningFileName, _warningCount));
        }
    }
}
