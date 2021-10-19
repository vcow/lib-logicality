using System;
using System.Collections.Generic;
using System.Linq;

namespace Base
{
	public sealed class ObservableImpl<T> : IDisposable, IObservable<T>
	{
		private bool _isDisposed;
		private bool _isStopped;
		private readonly object _observerLock = new object();
		private readonly List<IObserver<T>> _observers = new List<IObserver<T>>();

		private class Subscription : IDisposable
		{
			private bool _isDisposed;
			private ObservableImpl<T> _parent;
			private IObserver<T> _observer;

			public Subscription(ObservableImpl<T> parent, IObserver<T> observer)
			{
				_parent = parent;
				_observer = observer;
			}

			public void Dispose()
			{
				if (_isDisposed) return;
				_isDisposed = true;

				lock (_parent._observerLock)
				{
					_parent._observers.Remove(_observer);
				}

				_parent = null;
				_observer = null;
			}
		}

		private class EmptySubscription : IDisposable
		{
			public void Dispose()
			{
			}
		}

		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			lock (_observerLock)
			{
				_observers.Clear();
			}
		}

		public IDisposable Subscribe(IObserver<T> observer)
		{
			if (observer == null)
			{
				throw new ArgumentNullException();
			}

			if (_isDisposed)
			{
				return new EmptySubscription();
			}

			Subscription subscription;
			lock (_observerLock)
			{
				if (_isStopped) return new EmptySubscription();

				_observers.Add(observer);
				subscription = new Subscription(this, observer);
			}

			return subscription;
		}

		public void OnNext(T value)
		{
			if (_isDisposed || _isStopped) return;

			List<IObserver<T>> observers;
			lock (_observerLock)
			{
				observers = _observers.ToList();
			}

			observers.ForEach(observer => observer.OnNext(value));
		}

		public void OnError(Exception e)
		{
			if (_isDisposed || _isStopped) return;

			List<IObserver<T>> observers;
			lock (_observerLock)
			{
				if (_isStopped) return;

				observers = _observers.ToList();
				_observers.Clear();
				_isStopped = true;
			}

			observers.ForEach(observer => observer.OnError(e));
		}

		public void OnCompleted()
		{
			if (_isDisposed || _isStopped) return;

			List<IObserver<T>> observers;
			lock (_observerLock)
			{
				if (_isStopped) return;

				observers = _observers.ToList();
				_observers.Clear();
				_isStopped = true;
			}

			observers.ForEach(observer => observer.OnCompleted());
		}
	}
}