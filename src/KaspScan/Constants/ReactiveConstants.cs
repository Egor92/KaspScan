using System;

namespace KaspScan.Constants
{
    public static class ReactiveConstants
    {
        public static readonly TimeSpan TextChangedDelay = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan ItemChangedDelay = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan ItemChangedLongDelay = TimeSpan.FromMilliseconds(200);
        public static readonly TimeSpan ActivationDelay = TimeSpan.FromMilliseconds(150);
        public static readonly TimeSpan ActivationLongDelay = TimeSpan.FromMilliseconds(500);
        public static readonly TimeSpan BusyIndicatorDelay = TimeSpan.FromMilliseconds(200);
        public static readonly TimeSpan ScrollingDelay = TimeSpan.FromMilliseconds(100);
    }
}
