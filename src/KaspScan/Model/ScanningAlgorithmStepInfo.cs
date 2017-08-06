namespace KaspScan.Model
{
    public class ScanningAlgorithmStepInfo
    {
        public ScanningAlgorithmStepInfo(AlgorithmStatus status, double progress, string actualScanningFileName, int warningCount)
        {
            Status = status;
            Progress = progress;
            ActualScanningFileName = actualScanningFileName;
            WarningCount = warningCount;
        }

        public AlgorithmStatus Status { get; }

        public double Progress { get; }

        public string ActualScanningFileName { get; }

        public int WarningCount { get; }
    }
}
