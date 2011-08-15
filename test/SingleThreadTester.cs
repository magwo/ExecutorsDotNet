using System;
using NUnit.Framework;
using Executors;
namespace Executors.Test
{
	[TestFixture]
	public class SingleThreadTester : CommonTester
	{
		public override IExecutor CreateDefaultExecutor(ShutdownMode shutdownMode) {
			return new SingleThreadExecutor(shutdownMode);
		}
	}
}

