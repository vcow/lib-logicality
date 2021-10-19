using System;

namespace Base.GameTask
{
	/// <summary>
	/// Задача.
	/// </summary>
	public interface IGameTask
	{
		/// <summary>
		/// Запуск задачи на исполнение.
		/// </summary>
		void Start();

		/// <summary>
		/// Флаг завершения задачи.
		/// </summary>
		bool Completed { get; }

		/// <summary>
		/// Поток изменения состояния завершения задачи.
		/// </summary>
		IObservable<bool> CompletedChangesStream { get; }
	}
}