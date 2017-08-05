using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using ReactiveUI;

namespace KaspScan.ViewModels.Base
{
    public abstract class DisposableViewModel : ReactiveObject, IDisposable
    {
        #region Properties

        #region Disposables

        protected CompositeDisposable Disposables { get; } = new CompositeDisposable();

        #endregion

        #region DisposableEnumerables

        private ICollection<IEnumerable> DisposableEnumerables { get; } = new Collection<IEnumerable>();

        #endregion

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
                Disposables.Dispose();

                foreach (var disposableEnumerable in DisposableEnumerables)
                {
                    foreach (var disposable in disposableEnumerable.OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }
                }
            }

            this.RaisePropertyChanged("IsDisposed");
        }

        #endregion
    }
}
