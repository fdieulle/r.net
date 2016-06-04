using System.Collections.Generic;
using NUnit.Framework;
using RDotNet.ClrProxy.Loggers;

namespace RDotNet.ClrProxyTests
{
    [TestFixture]
    public class LoggerTests
    {
        private MockLogger mockLogger;

        [SetUp]
        public void Setup()
        {
            mockLogger = new MockLogger();
            Logger.Instance.AddLogger(mockLogger);
        }

        [TearDown]
        public void Teardown()
        {
            Logger.Instance.RemoveLogger(mockLogger);
        }

        [Test]
        public void Test()
        {
            var logger = Logger.Instance;

            logger.Info("Test Info");
            Assert.AreEqual("Test Info", mockLogger.infoQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Warn("Test Warn");
            Assert.AreEqual("Test Warn", mockLogger.warnQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Error("Test Error");
            Assert.AreEqual("Test Error", mockLogger.errorQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Debug("Test Debug");
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Level = LogLevel.Debug;
            Assert.IsTrue(mockLogger.IsDebugEnabled);
            Assert.IsTrue(mockLogger.IsInfoEnabled);
            Assert.IsTrue(mockLogger.IsWarnEnabled);
            Assert.IsTrue(mockLogger.IsErrorEnabled);

            logger.Info("Test Info");
            Assert.AreEqual("Test Info", mockLogger.infoQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Warn("Test Warn");
            Assert.AreEqual("Test Warn", mockLogger.warnQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Error("Test Error");
            Assert.AreEqual("Test Error", mockLogger.errorQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);

            logger.Debug("Test Debug");
            Assert.AreEqual("Test Debug", mockLogger.debugQueue.Dequeue());
            Assert.AreEqual(0, mockLogger.debugQueue.Count);
            Assert.AreEqual(0, mockLogger.infoQueue.Count);
            Assert.AreEqual(0, mockLogger.warnQueue.Count);
            Assert.AreEqual(0, mockLogger.errorQueue.Count);
        }

        private class MockLogger : AbstractLogger
        {
            public readonly Queue<string> debugQueue = new Queue<string>();
            public readonly Queue<string> infoQueue = new Queue<string>();
            public readonly Queue<string> warnQueue = new Queue<string>();
            public readonly Queue<string> errorQueue = new Queue<string>();

            public MockLogger() : base("Mock")
            {

            }

            #region Overrides of AbstractLogger

            protected override void DebugOverride(string message)
            {
                debugQueue.Enqueue(message);
            }

            protected override void InfoOverride(string message)
            {
                infoQueue.Enqueue(message);
            }

            protected override void WarnOverride(string message)
            {
                warnQueue.Enqueue(message);
            }

            protected override void ErrorOverride(string message)
            {
                errorQueue.Enqueue(message);
            }

            #endregion
        }
    }
}
