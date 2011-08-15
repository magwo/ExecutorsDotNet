using System;
using NUnit.Framework;
using Executors;
using System.Collections.Generic;
using System.Threading;

namespace Executors.Test
{
	[TestFixture]
	public abstract class CommonTester
	{
		public IExecutor CreateDefaultExecutor() {
			return CreateDefaultExecutor(ShutdownMode.FinishAll);
		}
		public abstract IExecutor CreateDefaultExecutor(ShutdownMode shutdownMode);

		class AddTask : ICallable<int>
		{
			int a, b;
			public AddTask(int a, int b)
			{
				this.a = a;
				this.b = b;
			}
			public int Call()
			{
				return a + b;
			}
		}

		class DummyException : Exception
		{
		}
		class ExceptionThrowingTask : ICallable<int>
		{
			public int Call()
			{
				throw new DummyException();
			}
		}

		class SleepingTask : ICallable<int>
		{
			int sleepDuration;

			public SleepingTask(int sleepDuration)
			{
				this.sleepDuration = sleepDuration;
			}

			public int Call()
			{
				Thread.Sleep(sleepDuration);
				return 0;
			}
		}


		[Test]
		public void ShouldCompleteTask()
		{
			var executor = CreateDefaultExecutor();
			var future = executor.Submit<int>(new AddTask(5, 6));
			Assert.AreEqual(11, future.GetResult());
		}

		[Test]
		public void ShouldCompleteAllTasks()
		{
			var executor = CreateDefaultExecutor();
			var tasks = new List<AddTask>();
			var futures = new List<Future<int>>();
			
			for(int i = 0; i < 10; i++) {
				var task = new AddTask(i, i + 1);
				tasks.Add(task);
				futures.Add(executor.Submit<int>(task));
			}
			for(int i = 0; i < tasks.Count; i++) {
				Assert.AreEqual(tasks[i].Call(), futures[i].GetResult());
			}
			
			Assert.AreEqual(0, executor.GetQueueSize());
		}

		[Test]
		[ExpectedException(typeof(ExecutionException))]
		public void ShouldRethrowTaskException()
		{
			var executor = CreateDefaultExecutor();
			executor.Submit<int>(new ExceptionThrowingTask()).GetResult();
		}
		
		[Test]
		public void ShouldCompleteAllTasksOnShutdown() {
			var executor = CreateDefaultExecutor(ShutdownMode.FinishAll);
			var futures = new List<Future<int>>();
			for(int i=0; i<10; i++) {
				futures.Add(executor.Submit(new SleepingTask(20)));
			}
			executor.Shutdown();
			while(!executor.IsShutdown()) { }
			foreach(Future<int> future in futures) {
				Assert.That(future.IsDone);
			}
			Assert.AreEqual(0, executor.GetQueueSize());
		}
	}
}

