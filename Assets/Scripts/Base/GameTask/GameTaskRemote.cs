using System;
using UnityEngine.Assertions;

namespace Base.GameTask
{
	/// <inheritdoc cref="Base.GameTask.IGameTask" />
	/// <summary>
	/// Отложенная задача. Принимает замыкание, которое будет вызвано в момент старта задачи.
	/// </summary>
	public class GameTaskRemote : IGameTask, IDisposable
	{
		private bool _isDisposed;
		private bool _completed;
		private readonly Func<IGameTask> _closure;
		private readonly ObservableImpl<bool> _completedChangesStream = new ObservableImpl<bool>();
		private IDisposable _completedHandler;
		private IGameTask _gameTask;

		public GameTaskRemote(Func<IGameTask> closure)
		{
			_closure = closure;
		}

		public bool Completed
		{
			get => _completed;
			private set
			{
				if (value == _completed) return;
				_completed = value;

				Assert.IsTrue(_completed);
				_completedChangesStream.OnNext(_completed);
				_completedChangesStream.OnCompleted();
			}
		}

		public IObservable<bool> CompletedChangesStream => _completedChangesStream;

		// ITask

		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			_completedHandler?.Dispose();
			_completedHandler = null;

			_completedChangesStream.Dispose();

			_gameTask = null;
		}

		public void Start()
		{
			if (_isDisposed || _gameTask != null || Completed) return;

			_gameTask = _closure.Invoke();
			if (_gameTask != null)
			{
				if (_gameTask.Completed)
				{
					SubTaskCompleteHandler(true);
				}
				else
				{
					_completedHandler = _gameTask.CompletedChangesStream
						.Subscribe(new ObserverImpl<bool>(SubTaskCompleteHandler));
					_gameTask.Start();
				}
			}
			else
			{
				Completed = true;
			}
		}

		// \ITask

		private void SubTaskCompleteHandler(bool result)
		{
			if (!result) return;

			_completedHandler?.Dispose();
			_completedHandler = null;

			_gameTask = null;
			Completed = true;
		}
	}
}