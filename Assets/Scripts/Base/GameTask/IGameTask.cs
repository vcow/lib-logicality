﻿using System;

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
		/// Событие завершения задачи.
		/// </summary>
		event EventHandler CompleteEvent;
	}
}