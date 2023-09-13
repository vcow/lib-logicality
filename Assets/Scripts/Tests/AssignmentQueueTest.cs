using Base.Assignments;
using NUnit.Framework;

namespace Tests
{
	public class AssignmentQueueTest
	{
		/// <summary>
		/// Test result of the sequentially Assignments.
		/// </summary>
		[Test]
		public void AssignmentQueueTestResult()
		{
			// ((50 + 50) / 4 + 5) * 3 + 10 = 100
			var queue = new AssignmentQueue();
			var value = new TestAssignmentValue<int>(50);

			queue.Add(new TestAssignment<int>("step1(+50)", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 50);
			}));
			queue.Add(new TestAssignment<int>("step2(/4)", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v / 4);
			}));
			queue.Add(new TestAssignment<int>("step3(+5)", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 5);
			}));
			queue.Add(new TestAssignment<int>("step4(*3)", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v * 3);
			}));
			queue.Add(new TestAssignment<int>("step5(+10)", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 10);
			}));

			queue.CompleteEvent += assignment =>
			{
				queue.Dispose();
				Assert.AreEqual(value.GetValue(), 100);
			};
			queue.Start();
		}

		/// <summary>
		/// Test result of the sequentially Assignments with delay.
		/// </summary>
		[Test]
		public void AssignmentQueueWithDelayedItemsTestResult()
		{
			// ((50 + 50) / 4 + 5) * 3 + 10 = 100
			var queue = new AssignmentQueue();
			var value = new TestAssignmentValue<int>(50);

			queue.Add(new TestAssignment<int>("delayed_step1(+50)_10ms", value, 10, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 50);
			}));
			queue.Add(new TestAssignment<int>("delayed_step2(/4)_0ms", value, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v / 4);
			}));
			queue.Add(new TestAssignment<int>("delayed_step3(+5)_25ms", value, 25, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 5);
			}));
			queue.Add(new TestAssignment<int>("delayed_step4(*3)_1ms", value, 1, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v * 3);
			}));
			queue.Add(new TestAssignment<int>("delayed_step5(+10)_64ms", value, 64, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 10);
			}));

			queue.CompleteEvent += assignment =>
			{
				queue.Dispose();
				Assert.AreEqual(value.GetValue(), 100);
			};
			queue.Start();
		}
	}
}