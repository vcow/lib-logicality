using System;
using Base.GameTask;
using UnityEngine;
using UnityEngine.Assertions;

namespace Base.GameService
{
	/// <summary>
	/// Вспомогательная задача на инициализацию сервиса.
	/// </summary>
	public class GameTaskInitService : IGameTask, IDisposable
	{
		private bool _completed;
		private readonly IGameService _gameService;
		private readonly object[] _args;
		private readonly ObservableImpl<bool> _completedChangesStream = new ObservableImpl<bool>();
		private IDisposable _gameServiceReadyHandler;

		private bool _isDisposed;
		private bool _isStarted;

		public GameTaskInitService(IGameService gameService, object[] args = null)
		{
			_gameService = gameService ?? throw new ArgumentNullException();
			_args = args ?? Array.Empty<object>();
		}

		// ITask

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

		public void Start()
		{
			if (_isStarted || _isDisposed) return;
			_isStarted = true;

			if (_gameService.IsReady)
			{
				Debug.LogError("The Service is ready when the Task starts.");
				Completed = true;
				return;
			}

			_gameServiceReadyHandler = _gameService.IsReadyChangesStream
				.Subscribe(new ObserverImpl<bool>(b =>
				{
					if (!b) return;
					_gameServiceReadyHandler?.Dispose();
					_gameServiceReadyHandler = null;
					Completed = true;
				}));
			_gameService.Initialize(_args);
		}

		// \ITask

		// IDisposable

		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			_gameServiceReadyHandler?.Dispose();
			_gameServiceReadyHandler = null;
		}

		// \IDisposable
	}
}