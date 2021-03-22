using System;
using UnityEngine.Assertions;

namespace Base.GameTask
{
	/// <inheritdoc />
	/// <summary>
	/// Отложенная задача. Принимает замыкание, которое будет вызвано в момент старта задачи.
	/// </summary>
	public class GameTaskRemote : IGameTask
	{
		private bool _completed;
		private readonly Func<IGameTask> _closure;

		private IDisposable _completeHandler;
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

				Assert.IsFalse(_completed);
				_completed = value;
				CompleteEvent?.Invoke(this, new ReadyEventArgs(true));
			}
		}

		public event EventHandler<ReadyEventArgs> CompleteEvent;

		// ITask

		public void Start()
		{
			if (_gameTask != null || Completed) return;

			_gameTask = _closure.Invoke();
			if (_gameTask != null)
			{
				_gameTask.CompleteEvent += SubTaskCompleteHandler;
				_gameTask.Start();
			}
			else
			{
				Completed = true;
			}
		}

		// \ITask

		private void SubTaskCompleteHandler(object sender, EventArgs args)
		{
			var task = (IGameTask) sender;
			task.CompleteEvent -= SubTaskCompleteHandler;
			_gameTask = null;
			Completed = true;
		}
	}
}