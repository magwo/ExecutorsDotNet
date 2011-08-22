using System;
using System.Threading;
namespace Executors.Test
{

	public class SummationTask : ICallable<int>
	{
		int a, b;
		public SummationTask(int a, int b)
		{
			this.a = a;
			this.b = b;
		}
		public int Call()
		{
			return a + b;
		}
	}
	
	public class DummyException : Exception
	{
	}
	public class ExceptionThrowingTask : ICallable<int>
	{
		public int Call()
		{
			throw new DummyException();
		}
	}
	
	public class SleepingTask : ICallable<int>
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
	
	
	public class WaitingTask : ICallable<int>
	{
		
		public int Call() {
			lock(this) {
				
			}
			return 0;
		}
	}
}

