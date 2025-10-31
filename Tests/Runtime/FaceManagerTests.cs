using NUnit.Framework;
using UnityEngine;

namespace FaceLib.Tests
{
    public class FaceManagerTests
    {
        [SetUp]
        public void SetUp()
        {
            // Ensure a clean state before each test
            FaceLib.FaceManager.EditorForceResetForTests();
        }

        [Test]
        public void EnsureExists_DoesNotThrow_And_CreatesInstance()
        {
            Assert.DoesNotThrow(() => FaceLib.FaceManager.EnsureExists());
            Assert.IsNotNull(FaceLib.FaceManager.Instance);
        }

        [Test]
        public void Initialize_Sets_IsInitialized_True()
        {
            FaceLib.FaceManager.EnsureExists();
            Assert.IsFalse(FaceLib.FaceManager.IsInitialized);
            FaceLib.FaceManager.Initialize();
            Assert.IsTrue(FaceLib.FaceManager.IsInitialized);
        }

        [Test]
        public void Shutdown_Sets_IsInitialized_False()
        {
            FaceLib.FaceManager.EnsureExists();
            FaceLib.FaceManager.Initialize();
            FaceLib.FaceManager.Shutdown();
            Assert.IsFalse(FaceLib.FaceManager.IsInitialized);
        }
    }
}


