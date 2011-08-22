using System;
using NUnit.Framework;
using Executors;
using System.Collections.Generic;
using System.Threading;
namespace Executors.Test
{
	[TestFixture]
	public class SingleThreadTester : CommonTester
	{
		public override IExecutor CreateDefaultExecutor(ShutdownMode shutdownMode) {
			return new SingleThreadExecutor(shutdownMode);
		}
		
		
		[Test]
		public void ShouldCancelTasksOnShutdown() {
			var executor = CreateDefaultExecutor(ShutdownMode.CancelQueuedTasks);
			var tasks = new List<WaitingTask>();
			var futures = new List<Future<int>>();
			for(int i=0; i<10; i++) {
				WaitingTask task = new WaitingTask();
				tasks.Add(task);
				if(i == 0) {
					Monitor.Enter(task);
				}
				futures.Add(executor.Submit(task));
			}
			executor.Shutdown();
			Monitor.Exit(tasks[0]);
			
			while(!executor.IsShutdown()) { }
			Assert.AreEqual(0, futures[0].GetResult());
			foreach(Future<int> future in futures.GetRange(1, 9)) {
				Assert.IsTrue(future.IsDone);
				try {
					Assert.AreEqual(0, future.GetResult());
					Assert.Fail(); // Should not reach this
				} catch(ExecutionException e) {
					Assert.IsInstanceOfType(typeof(TaskCancelledException), e.delayedException);
				}
			}
		}
	}
}

