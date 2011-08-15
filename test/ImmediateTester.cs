using System;
using NUnit.Framework;
using Executors;
namespace Executors.Test
{
	[TestFixture]
	public class ImmediateTester : CommonTester
	{
		
		public override IExecutor CreateDefaultExecutor(ShutdownMode shutdownMode) {
			return new ImmediateExecutor();
		}
	}
}

