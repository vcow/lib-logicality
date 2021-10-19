using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions;

namespace Base.GameTask
{
	/// <inheritdoc cref="IGameTask" />
	/// <summary>
	/// Параллельное выполнение задач.
	/// </summary>
	public class GameTaskConcurent : IGameTask, IDisposable
	{
		private bool _completed;
		private bool _isStarted;
		private readonly List<IGameTask> _tasks = new List<IGameTask>();
		private readonly List<IDisposable> _subTaskCompleteHandlers = new List<IDisposable>();
		private readonly Mutex _completeMutex = new Mutex();
		private readonly ObservableImpl<bool> _completedChangesStream = new ObservableImpl<bool>();

		private bool _isDisposed;

		// ITask

		public void Start()
		{
			if (_isStarted || Completed || _isDisposed) return;

			_isStarted = true;
			if (_tasks.Count > 0)
			{
				_tasks.ToList().ForEach(task =>
				{
					if (task.Completed)
					{
						Debug.LogWarning("Task in Concurent already completed.");
						_tasks.Remove(task);
						return;
					}

					IDisposable handler = null;
					handler = task.CompletedChangesStream
						.Subscribe(new ObserverImpl<bool>(b =>
						{
							if (!b) return;

							var completed = false;
							// ReSharper disable AccessToModifiedClosure
							if (_completeMutex.WaitOne())
							{
								Assert.IsNotNull(handler);
								handler.Dispose();
								_subTaskCompleteHandlers.Remove(handler);
								completed = _subTaskCompleteHandlers.Count <= 0;
								_completeMutex.ReleaseMutex();
							}
							// ReSharper restore AccessToModifiedClosure

							if (completed) Completed = true;
						}));
					_subTaskCompleteHandlers.Add(handler);
				});

				if (_tasks.Count > 0)
				{
					_tasks.ToList().ForEach(task => task.Start());
					_tasks.Clear();
				}
				else
				{
					Completed = true;
				}
			}
			else
			{
				Completed = true;
			}
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

			_subTaskCompleteHandlers.ForEach(disposable => disposable.Dispose());
			_subTaskCompleteHandlers.Clear();

			_tasks.ForEach(task => (task as IDisposable)?.Dispose());
			_tasks.Clear();
		}

		// \IDisposable

		/// <summary>
		/// Очистить параллельное выполнение.
		/// </summary>
		public void Clear()
		{
			if (_isDisposed) return;

			_subTaskCompleteHandlers.ForEach(disposable => disposable.Dispose());
			_subTaskCompleteHandlers.Clear();

			_tasks.Clear();
		}

		/// <summary>
		/// Добавить задачу в параллельное выполнение.
		/// </summary>
		/// <param name="gameTask">Добавляемая задача.</param>
		/// <exception cref="Exception">Параллельное выполнение уже запущено.</exception>
		public void Add(IGameTask gameTask)
		{
			if (_isDisposed) return;

			if (_isStarted) throw new Exception("Concurent already executed.");
			Assert.IsFalse(Completed, "Concurent already completed, added task will have no effect.");
			_tasks.Add(gameTask);
		}
	}
}