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
        private const long TotalStepCount = 500;
        private long _currentStepIndex;
        private bool _generateWarnings;
        private int _warningCount;
        private static readonly FileScannedInfo DefaultFileScannedInfo = new FileScannedInfo(AlgorithmStatus.NotRunned, 0.0, null, 0);

        #endregion

        #region Properties

        #region Status

        public AlgorithmStatus Status { get; private set; } = AlgorithmStatus.NotRunned;

        #endregion

        #endregion

        #region Events

        #region FileScanned

        private readonly BehaviorSubject<FileScannedInfo> _fileScanned = new BehaviorSubject<FileScannedInfo>(DefaultFileScannedInfo);

        public IObservable<FileScannedInfo> FileScanned
        {
            get { return _fileScanned.AsObservable(); }
        }

        #endregion

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            Stop();
            _intervalSubscription?.Dispose();
            _fileScanned?.Dispose();
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
                _currentStepIndex = 0;
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
            _currentStepIndex++;
            var progress = (double) _currentStepIndex / TotalStepCount * 100;
            var actualScanningFileName = RandomStringHelper.GetWord(5, 15);
            if (_generateWarnings && RandomHelper.GetBool(0.01))
            {
                _warningCount++;
            }
            _fileScanned.OnNext(new FileScannedInfo(Status, progress, actualScanningFileName, _warningCount));

            if (_currentStepIndex >= TotalStepCount)
            {
                _intervalSubscription.Dispose();
                Status = AlgorithmStatus.Finished;
                _fileScanned.OnNext(new FileScannedInfo(Status, progress, null, _warningCount));
            }
        }
    }
}
