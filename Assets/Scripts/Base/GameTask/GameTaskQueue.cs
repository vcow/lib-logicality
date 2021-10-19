using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace Base.GameTask
{
	/// <inheritdoc cref="IGameTask" />
	/// <summary>
	/// Очередь задач.
	/// </summary>
	public class GameTaskQueue : IGameTask, IDisposable
	{
		private bool _completed;
		private readonly Queue<IGameTask> _queue = new Queue<IGameTask>();
		private readonly ObservableImpl<bool> _completedChangesStream = new ObservableImpl<bool>();
		private readonly Mutex _queueMutex = new Mutex();

		private IGameTask _currentGameTask;
		private IDisposable _currentGameTaskCompleteHandler;

		private bool _isDisposed;

		// ITask

		public void Start()
		{
			if (_currentGameTask != null || Completed || _isDisposed) return;
			StartNextTask();
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

		// \ITask

		// IDisposable

		public void Dispose()
		{
			if (_isDisposed) return;
			_isDisposed = true;

			if (_queueMutex.WaitOne())
			{
				foreach (var task in _queue)
				{
					(task as IDisposable)?.Dispose();
				}

				_queue.Clear();
				_queueMutex.ReleaseMutex();
			}

			_currentGameTaskCompleteHandler?.Dispose();
			_currentGameTaskCompleteHandler = null;

			if (_currentGameTask != null)
			{
				(_currentGameTask as IDisposable)?.Dispose();
				_currentGameTask = null;
			}

			_completedChangesStream.Dispose();
		}

		// \IDisposable

		/// <summary>
		/// Очистить очередь.
		/// </summary>
		public void Clear()
		{
			if (_isDisposed) return;

			if (_queueMutex.WaitOne())
			{
				_queue.Clear();
				_queueMutex.ReleaseMutex();
			}

			_currentGameTaskCompleteHandler?.Dispose();
			_currentGameTaskCompleteHandler = null;
			_currentGameTask = null;
		}

		/// <summary>
		/// Добавить задачу в очередь.
		/// </summary>
		/// <param name="gameTask">Добавляемая задача.</param>
		public void Add(IGameTask gameTask)
		{
			if (_isDisposed) return;

			Assert.IsFalse(Completed, "Queue already completed, added task will have no effect.");
			if (_queueMutex.WaitOne())
			{
				_queue.Enqueue(gameTask);
				_queueMutex.ReleaseMutex();
			}
		}

		private void StartNextTask()
		{
			_currentGameTaskCompleteHandler?.Dispose();
			_currentGameTaskCompleteHandler = null;

			if (_queueMutex.WaitOne())
			{
				_currentGameTask = _queue.Count > 0 ? _queue.Dequeue() : null;
				_queueMutex.ReleaseMutex();
			}

			if (_currentGameTask == null)
			{
				Completed = true;
			}
			else if (_currentGameTask.Completed)
			{
				Debug.LogWarning("Task in Queue already completed.");
				StartNextTask();
			}
			else
			{
				_currentGameTaskCompleteHandler = _currentGameTask.CompletedChangesStream
					.Subscribe(new ObserverImpl<bool>(b =>
					{
						if (!b) return;
						StartNextTask();
					}));
				_currentGameTask.Start();
			}
		}
	}
}