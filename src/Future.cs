using System;
using System.Collections.Generic;
using System.Threading;

namespace Executors
{


	public interface IFuture
	{
		bool IsDone { get; }
	}


	/// <summary>
	/// A Future represents the result of a potentially asynchronous computation.
	/// Methods/properties are available to check if the operation is done or not.
	/// If an execption is thrown during the computation, this exception will be thrown
	/// when calling GetResult().
	/// </summary>
	/// <typeparam name="T">Type of the computation result object</typeparam>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public class Future<T> : IFuture
	{

		private T result;
		private Exception exception = null;

		volatile bool isDone = false;
		/// <summary>
		/// Is the computation done?
		/// </summary>
		public bool IsDone {
			get { return isDone; }
		}


		internal void SetResult(T result)
		{
			this.result = result;
		}

		internal void SetException(Exception e)
		{
			exception = e;
		}

		internal void SetDone()
		{
			isDone = true;
		}


		/// <summary>
		/// Get the result of the computation.
		/// Blocks until the computation is done.
		/// </summary>
		/// <todo>Add optional delegate parameter for waiting action</todo>
		public T GetResult()
		{
			// Could maybe do this with monitor instead.
			while(!IsDone) {
				Thread.Sleep(1);
			}
			
			if(exception != null) {
				throw exception;
			}
			
			return result;
		}
	}
}
