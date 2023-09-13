using System;
using System.Threading;
using Base.Assignments;
using UnityEngine;
using UnityEngine.Assertions;

namespace Tests
{
	/// <summary>
	/// Thread safe value for the TestAssignment.
	/// </summary>
	/// <typeparam name="T">The type of value.</typeparam>
	public class TestAssignmentValue<T> where T : struct
	{
		private readonly object _lock = new object();
		private T _value;

		public TestAssignmentValue(T initialValue)
		{
			_value = initialValue;
		}

		public T GetValue()
		{
			T value;
			lock (_lock)
			{
				value = _value;
			}

			return value;
		}

		public void SetValue(T value)
		{
			lock (_lock)
			{
				_value = value;
			}
		}
	}

	/// <summary>
	/// Simple multi-thread Assignment, that can modify given value with specified time delay.
	/// </summary>
	/// <typeparam name="T">A type of the modified value.</typeparam>
	public class TestAssignment<T> : IAssignment where T : struct
	{
		private bool _isStarted;
		private bool _completed;

		private readonly string _name;
		private readonly TestAssignmentValue<T> _value;
		private readonly Action<TestAssignmentValue<T>> _closure;
		private readonly int _msDelay;

		public TestAssignment(string name, TestAssignmentValue<T> value, int msDelay,
			Action<TestAssignmentValue<T>> closure)
		{
			_name = name;
			_value = value;
			_closure = closure;
			_msDelay = msDelay;
		}

		~TestAssignment()
		{
			Debug.LogFormat("The Assignment {0} completely destroyed.", _name);
		}

		public void Start()
		{
			if (_isStarted) return;
			_isStarted = true;

			var thread = new Thread(AssignmentThread);
			thread.Start(this);
		}

		public bool Completed
		{
			get => _completed;
			private set
			{
				if (value == _completed) return;
				_completed = value;
				Assert.IsTrue(_completed);
				CompleteEvent?.Invoke(this);
			}
		}

		public event AssignmentCompleteHandler CompleteEvent;

		private static void AssignmentThread(object instance)
		{
			var assignment = (TestAssignment<T>)instance;

			var delay = assignment._msDelay;
			if (delay > 0)
			{
				Thread.Sleep(delay);
			}

			assignment._closure.Invoke(assignment._value);
			assignment._isStarted = false;
			assignment.Completed = true;
		}
	}
}