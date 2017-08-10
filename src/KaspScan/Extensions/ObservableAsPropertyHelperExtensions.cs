using ReactiveUI;

namespace KaspScan.Extensions
{
    public static class ObservableAsPropertyHelperExtensions
    {
        public static T GetValueOrDefault<T>(this ObservableAsPropertyHelper<T> observableAsPropertyHelper,
                                             T defaultValue = default(T))
        {
            if (observableAsPropertyHelper == null)
                return defaultValue;

            return observableAsPropertyHelper.Value;
        }
    }
}
