using System;
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
        private readonly TimeSpan _algorithmStepInterval = TimeSpan.FromMilliseconds(10);
        private const long MaxAlgorithmStep = 500;
        private long _currentAlgorithmStep;
        private bool _generateWarnings;
        private int _warningCount;

        #endregion

        #region Properties

        #region Status

        public AlgorithmStatus Status { get; private set; } = AlgorithmStatus.NotRunned;

        #endregion

        #endregion

        #region Events

        #region StepPassed

        private readonly ReplaySubject<ScanningAlgorithmStepInfo> _stepPassed = new ReplaySubject<ScanningAlgorithmStepInfo>();

        public IObservable<ScanningAlgorithmStepInfo> StepPassed
        {
            get { return _stepPassed.AsObservable(); }
        }

        #endregion

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Stop();
            _intervalSubscription?.Dispose();
            _stepPassed?.Dispose();
        }

        #endregion

        #region Public methods

        public void Start()
        {
            lock (_syncRoot)
            {
                if (Status == AlgorithmStatus.Running)
                    return;

                Status = AlgorithmStatus.Running;
                _currentAlgorithmStep = 0;
                _warningCount = 0;
                _generateWarnings = RandomHelper.GetBool();
                _intervalSubscription = Observable.Interval(_algorithmStepInterval)
                                                  .Subscribe(OnNextAlgorithmStep);
            }
        }

        public void Pause()
        {
            lock (_syncRoot)
            {
                if (Status != AlgorithmStatus.Running)
                    return;

                Status = AlgorithmStatus.Paused;
                _intervalSubscription.Dispose();
            }
        }

        public void Stop()
        {
            lock (_syncRoot)
            {
                if (Status == AlgorithmStatus.Running || Status == AlgorithmStatus.Paused)
                    return;

                Status = AlgorithmStatus.Stopped;
                _intervalSubscription.Dispose();
            }
        }

        #endregion

        private void OnNextAlgorithmStep(long l)
        {
            _currentAlgorithmStep++;
            var progress = (double) _currentAlgorithmStep / MaxAlgorithmStep * 100;
            var actualScanningFileName = RandomStringHelper.GetWord(5, 15);
            if (_generateWarnings && RandomHelper.GetBool(0.01))
            {
                _warningCount++;
            }
            _stepPassed.OnNext(new ScanningAlgorithmStepInfo(Status, progress, actualScanningFileName, _warningCount));

            if (_currentAlgorithmStep >= MaxAlgorithmStep)
            {
                _intervalSubscription.Dispose();
                Status = AlgorithmStatus.Finished;
                _stepPassed.OnNext(new ScanningAlgorithmStepInfo(Status, progress, null, _warningCount));
            }
        }
    }
}
