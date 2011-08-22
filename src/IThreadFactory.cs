using System;
using System.Threading;
namespace Executors
{
	public interface IThreadFactory
	{
		Thread CreateOrGetThread(ThreadStart threadStart);
	}
	
	
	public class SimpleThreadFactory : IThreadFactory {
		public Thread CreateOrGetThread (ThreadStart threadStart)
		{
			return new Thread(threadStart);
		}
	}
}

