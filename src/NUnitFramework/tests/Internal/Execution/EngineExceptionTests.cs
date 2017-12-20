using System;
using System.Threading;

namespace NUnit.Framework.Internal.Execution
{
    public class EngineExceptionTests
    {
        [Test]
        public void CrashTheAgentProcess()
        {
            var thread = new Thread(ThrowUnhandledException);
            thread.Start();
            thread.Join();
        }

        [Test]
        public void BlahTest()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void LotsOfFail()
        {
            Assert.IsTrue(false);
        }

        void ThrowUnhandledException()
        {
            throw new Exception("Execute Order 66.");
        }
    }
}
