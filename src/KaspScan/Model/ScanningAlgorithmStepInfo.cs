namespace KaspScan.Model
{
    public class ScanningAlgorithmStepInfo
    {
        public ScanningAlgorithmStepInfo(double progress, string actualScanningFileName, int warningCount)
        {
            Progress = progress;
            ActualScanningFileName = actualScanningFileName;
            WarningCount = warningCount;
        }

        public double Progress { get; }
        public string ActualScanningFileName { get; }
        public int WarningCount { get; }
    }
}
