using Base.Assignments;
using NUnit.Framework;

namespace Tests
{
	public class AssignmentHybridTest
	{
		/// <summary>
		/// Test result of the hybrid sequentially and parallel Assignments.
		/// </summary>
		[Test]
		public void AssignmentHybridTestResult()
		{
			// 1 + 1 + 1 + 1 + 1 = 5
			var concurrent = new AssignmentConcurrent();

			var lockObject = new object();
			var aggregator = 0;

			var cValue = new TestAssignmentValue<int>(0);

			concurrent.Add(new TestAssignment<int>("assignment_1", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_2", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_3", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_4", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_5", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));

			// ((50 + 50) / 4 + cValue) * 3 + 10 = 100
			var queue = new AssignmentQueue();
			var qValue = new TestAssignmentValue<int>(50);

			queue.Add(new TestAssignment<int>("step1(+50)", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 50);
			}));
			queue.Add(new TestAssignment<int>("step2(/4)", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v / 4);
			}));
			queue.Add(concurrent);
			queue.Add(new TestAssignment<int>("step3(+cValue)", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + aggregator);
			}));
			queue.Add(new TestAssignment<int>("step4(*3)", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v * 3);
			}));
			queue.Add(new TestAssignment<int>("step5(+10)", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 10);
			}));

			queue.CompleteEvent += assignment =>
			{
				queue.Dispose();
				Assert.AreEqual(qValue.GetValue(), 100);
			};
			queue.Start();
		}

		/// <summary>
		/// Test result of the hybrid sequentially and parallel Assignments with delay.
		/// </summary>
		[Test]
		public void AssignmentHybridWithDelayedItemsTestResult()
		{
			// 1 + 1 + 1 + 1 + 1 = 5
			var concurrent = new AssignmentConcurrent();

			var lockObject = new object();
			var aggregator = 0;

			var cValue = new TestAssignmentValue<int>(0);

			concurrent.Add(new TestAssignment<int>("delayed_assignment_5ms", cValue, 5, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_0ms", cValue, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_30ms", cValue, 30, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_10ms", cValue, 10, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_5ms", cValue, 5, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 1;
				}
			}));

			// ((50 + 50) / 4 + cValue) * 3 + 10 = 100
			var queue = new AssignmentQueue();
			var qValue = new TestAssignmentValue<int>(50);

			queue.Add(new TestAssignment<int>("delayed_step1(+50)_10ms", qValue, 10, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 50);
			}));
			queue.Add(new TestAssignment<int>("delayed_step2(/4)_3ms", qValue, 3, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v / 4);
			}));
			queue.Add(concurrent);
			queue.Add(new TestAssignment<int>("delayed_step3(+cValue)_7ms", qValue, 7, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + aggregator);
			}));
			queue.Add(new TestAssignment<int>("delayed_step4(*3)_20ms", qValue, 20, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v * 3);
			}));
			queue.Add(new TestAssignment<int>("delayed_step5(+10)_0ms", qValue, 0, assignmentValue =>
			{
				var v = assignmentValue.GetValue();
				assignmentValue.SetValue(v + 10);
			}));

			queue.CompleteEvent += assignment =>
			{
				queue.Dispose();
				Assert.AreEqual(qValue.GetValue(), 100);
			};
			queue.Start();
		}
	}
}