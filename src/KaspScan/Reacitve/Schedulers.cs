using System;
using System.Reactive.Concurrency;
using System.Windows.Threading;

namespace KaspScan.Dependencies
{
    public interface ISchedulers
    {
        IScheduler MainThread { get; }
        IScheduler Background { get; }
        IScheduler ThreadPool { get; }
    }

    public class Schedulers : ISchedulers
    {
        #region Ctor

        public Schedulers(Dispatcher mainThreadDispatcher)
        {
            if (mainThreadDispatcher == null)
                throw new ArgumentNullException(nameof(mainThreadDispatcher));
            MainThread = new DispatcherScheduler(mainThreadDispatcher);
            Background = new EventLoopScheduler();
            ThreadPool = ThreadPoolScheduler.Instance;
        }

        #endregion

        #region Implementation of ISchedulers

        public IScheduler MainThread { get; }
        public IScheduler Background { get; }
        public IScheduler ThreadPool { get; }

        #endregion
    }
}
