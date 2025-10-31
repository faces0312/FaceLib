using NUnit.Framework;

namespace FaceLib.Tests
{
    public class FaceLoggerTests
    {
        [Test]
        public void FaceLogger_Log_DoesNotThrow()
        {
            Assert.DoesNotThrow(() => FaceLib.FaceLogger.Log("test"));
        }
    }
}


