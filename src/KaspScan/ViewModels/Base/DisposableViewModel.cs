using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Threading;
using ReactiveUI;

namespace KaspScan.ViewModels.Base
{
    public abstract class DisposableViewModel : ReactiveObject, IDisposable
    {
        #region Fields

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        #endregion

        #region Properties

        #region IsDisposed

        private int _isDisposed;

        public bool IsDisposed
        {
            get
            {
                Thread.MemoryBarrier();
                return _isDisposed == DisposedFlag;
            }
        }

        #endregion

        #endregion

        #region Implementation of IDisposable

        private const int DisposedFlag = 1;

        public void Dispose()
        {
            var wasDisposed = Interlocked.Exchange(ref _isDisposed, DisposedFlag);
            if (wasDisposed == DisposedFlag)
                return;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _disposables.Dispose();
            }

            this.RaisePropertyChanged("IsDisposed");
        }

        #endregion

        protected void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        protected void AddDisposables(IEnumerable<IDisposable> disposables)
        {
            foreach (var disposable in disposables)
            {
                _disposables.Add(disposable);
            }
        }
    }
}
