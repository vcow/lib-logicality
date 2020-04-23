using System;

namespace Base.GameService
{
	/// <summary>
	/// Интерфейс сущности, требующей начальной инициализации
	/// </summary>
	public interface IGameService
	{
		/// <summary>
		/// Начальная инициализация сервиса.
		/// </summary>
		/// <param name="args">Агрументы инициализации.</param>
		void Initialize(params object[] args);

		/// <summary>
		/// Флаг готовности сервиса.
		/// </summary>
		bool IsReady { get; }

		/// <summary>
		/// Событие готовности сервиса.
		/// </summary>
		event EventHandler ReadyEvent;
	}
}