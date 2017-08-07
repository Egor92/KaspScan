using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using KaspScan.Constants;
using KaspScan.Reacitve;
using Prism;
using ReactiveUI;

namespace KaspScan.ViewModels.Base
{
    public abstract class ActiveAwareViewModel : DisposableViewModel, IActiveAware
    {
        #region Fields

        private readonly ISchedulers _schedulers;
        private readonly ManualResetEvent _waitActivatingIsFinished = new ManualResetEvent(true);
        private bool _isActivated;
        private CompositeDisposable _subscriptionsOnActivation;

        #endregion

        #region Ctor

        protected ActiveAwareViewModel(ISchedulers schedulers)
        {
            _schedulers = schedulers;
            SubscribeToPropertyChanged();
        }

        private void SubscribeToPropertyChanged()
        {
            Disposables.Add(SubscribeToIsActiveChanged());
        }

        #endregion

        #region Properties

        #region ActivationDelay

        protected virtual TimeSpan ActivationDelay
        {
            get { return ReactiveConstants.ActivationDelay; }
        }

        #endregion

        #region IsActive

        private bool _isActive;

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (this.RaiseAndSetIfChanged(ref _isActive, value))
                    RaiseIsActiveChanged();
            }
        }

        [Obsolete("Do not use this event. Use ReactiveUI subscription!")]
        public event EventHandler IsActiveChanged;

        protected virtual void RaiseIsActiveChanged()
        {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
        }

        private IDisposable SubscribeToIsActiveChanged()
        {
            return this.ObservableForProperty(x => x.IsActive)
                       .Throttle(ActivationDelay, _schedulers.MainThread)
                       .Select(x => x.GetValue())
                       .Subscribe(OnIsActiveChanged);
        }

        private void OnIsActiveChanged(bool isActive)
        {
            Action action = isActive
                ? (Action)Activate
                : (Action)Deactivate;
            InvokeActivationAction(isActive, action);
        }

        private void InvokeActivationAction(bool isActive, Action action)
        {
            try
            {
                _waitActivatingIsFinished.WaitOne();

                if (_isActivated == isActive)
                    return;

                _isActivated = isActive;

                action?.Invoke();
            }
            finally
            {
                _waitActivatingIsFinished.Set();
            }
        }

        #endregion

        #endregion

        #region Virtual members

        protected virtual void OnActivating()
        {
        }

        protected virtual void OnDeactivating()
        {
        }

        protected virtual IEnumerable<IDisposable> GetSubscriptionsOnActivation()
        {
            yield break;
        }

        #endregion

        #region Overridden members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                DeactivateImmediately();
            }
        }

        #endregion

        private void DeactivateImmediately()
        {
            IsActive = false;
            InvokeActivationAction(false, Deactivate);
        }

        private void Activate()
        {
            SubscribeOnActivation();
            OnActivating();
        }

        private void SubscribeOnActivation()
        {
            var propertyChangedSubscriptions = GetSubscriptionsOnActivation();
            _subscriptionsOnActivation = new CompositeDisposable(propertyChangedSubscriptions);
        }

        private void Deactivate()
        {
            UnsubscribeOnDeactivation();
            OnDeactivating();
        }

        private void UnsubscribeOnDeactivation()
        {
            _subscriptionsOnActivation.Dispose();
            _subscriptionsOnActivation = null;
        }
    }
}