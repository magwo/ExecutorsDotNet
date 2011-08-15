using System;

namespace Executors
{
	/// <summary>
	/// Callable object that returns type T, and may throw an exception.
	/// </summary>
	/// <typeparam name="T">Type of the computation result object</typeparam>
	/// <author>Magnus Wolffelt, magnus.wolffelt@gmail.com</author>
	public interface ICallable<T>
	{
		T Call();
	}
}
