using System;

namespace Base.Activatable
{
	public enum ActivatableState
	{
		Inactive,
		Active,
		ToActive,
		ToInactive
	}

	public interface IActivatable
	{
		/// <summary>
		/// Текущее состояние активируемого объекта.
		/// </summary>
		ActivatableState ActivatableState { get; }

		/// <summary>
		/// Поток изменения текущего состояния.
		/// </summary>
		IObservable<ActivatableState> ActivatableStateChangesStream { get; }

		/// <summary>
		/// Активировать объект.
		/// </summary>
		/// <param name="immediately">Флаг, указывающий активировать объект немедленно.</param>
		void Activate(bool immediately = false);

		/// <summary>
		/// Деактивировать объект.
		/// </summary>
		/// <param name="immediately">Флаг, указывающий деактивировать объект немедленно.</param>
		void Deactivate(bool immediately = false);
	}
}