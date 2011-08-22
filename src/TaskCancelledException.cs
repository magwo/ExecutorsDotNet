using System;
namespace Executors
{
	public class TaskCancelledException : Exception
	{
		public TaskCancelledException(string message) : base(message) { }
	}
}

