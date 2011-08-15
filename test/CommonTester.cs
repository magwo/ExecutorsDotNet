using System;
using NUnit.Framework;
using Executors;
using System.Collections.Generic;
namespace Executors.Test
{
	[TestFixture]
	public abstract class CommonTester
	{
		public abstract IExecutor CreateDefaultExecutor();
		
		public class AddTask : ICallable<int> {
			public int a, b;
			public AddTask(int a, int b) {
				this.a = a;
				this.b = b;
			}
			public int Call()
			{
				return a + b;
			}
		}
		

		[Test]
		public void ShouldCompleteTask() {
			var executor = CreateDefaultExecutor();
			var future = executor.Submit<int>(new AddTask(5,6));
			Assert.AreEqual(11, future.GetResult());
		}
		
		
		[Test]
		public void ShouldCompleteAllTasks() {
			var executor = CreateDefaultExecutor();
			var tasks = new List<AddTask>();
			var futures = new List<Future<int>>();
			
			for(int i=0; i<10; i++) {
				var task = new AddTask(i, i+1);
				tasks.Add(task);
				futures.Add(executor.Submit<int>(task));
			}
			for(int i=0; i<tasks.Count; i++) {
				Assert.AreEqual(tasks[i].Call(), futures[i].GetResult());
			}
			
			Assert.AreEqual(0, executor.GetQueueSize());
		}
	}
}

