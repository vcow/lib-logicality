using Base.Assignments;
using NUnit.Framework;

namespace Tests
{
	public class AssignmentConcurrentTest
	{
		/// <summary>
		/// Test result of the parallel Assignments.
		/// </summary>
		[Test]
		public void AssignmentConcurrentTestResult()
		{
			// 10000 + 2000 + 300 + 40 + 5 = 12345
			var concurrent = new AssignmentConcurrent();

			var lockObject = new object();
			var aggregator = 0;

			var value = new TestAssignmentValue<int>(0);

			concurrent.Add(new TestAssignment<int>("assignment_1", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 10000;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_2", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 2000;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_3", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 300;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_4", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 40;
				}
			}));
			concurrent.Add(new TestAssignment<int>("assignment_5", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 5;
				}
			}));

			concurrent.CompleteEvent += assignment =>
			{
				concurrent.Dispose();
				Assert.AreEqual(aggregator, 12345);
			};
			concurrent.Start();
		}

		/// <summary>
		/// Test result of the parallel Assignments with delay.
		/// </summary>
		[Test]
		public void AssignmentConcurrentWithDelayedItemsTestResult()
		{
			// 10000 + 2000 + 300 + 40 + 5 = 12345
			var concurrent = new AssignmentConcurrent();

			var lockObject = new object();
			var aggregator = 0;

			var value = new TestAssignmentValue<int>(0);

			concurrent.Add(new TestAssignment<int>("delayed_assignment_0ms", value, 0, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 10000;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_10ms", value, 10, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 2000;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_10ms", value, 10, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 300;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_100ms", value, 100, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 40;
				}
			}));
			concurrent.Add(new TestAssignment<int>("delayed_assignment_25ms", value, 25, assignmentValue =>
			{
				lock (lockObject)
				{
					aggregator += 5;
				}
			}));

			concurrent.CompleteEvent += assignment =>
			{
				concurrent.Dispose();
				Assert.AreEqual(aggregator, 12345);
			};
			concurrent.Start();
		}
	}
}