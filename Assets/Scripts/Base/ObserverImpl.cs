using System;

namespace Base
{
	public sealed class ObserverImpl<T> : IObserver<T>
	{
		private readonly Action<T> _onNext;
		private readonly Action<Exception> _onError;
		private readonly Action _onCompleted;

		public ObserverImpl(Action<T> onNext, Action<Exception> onError = null, Action onCompleted = null)
		{
			_onNext = onNext ?? throw new ArgumentNullException();
			_onError = onError;
			_onCompleted = onCompleted;
		}

		void IObserver<T>.OnCompleted()
		{
			_onCompleted?.Invoke();
		}

		void IObserver<T>.OnError(Exception e)
		{
			_onError?.Invoke(e);
		}

		void IObserver<T>.OnNext(T value)
		{
			_onNext(value);
		}
	}
}