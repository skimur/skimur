using System;
using System.IO;
using NUnit.Framework;
using Skimur.FileSystem;

namespace Skimur.Tests
{
    [TestFixture]
    public class LocalFileSystemTests
    {
        private IFileSystem _fileSystem;

        [Test]
        public void Can_manage_directories()
        {
            var testDirectory1 = _fileSystem.GetDirectory("test");
            Assert.That(testDirectory1.Exists, Is.False);
            testDirectory1.Create();
            Assert.That(testDirectory1.Exists, Is.True);
            testDirectory1.Delete();
            Assert.That(testDirectory1.Exists, Is.False);

            var testDirectory2 = _fileSystem.GetDirectory("test/directory");
            Assert.That(testDirectory2.Exists, Is.False);
            testDirectory2.Create();
            Assert.That(testDirectory2.Exists, Is.True);
            testDirectory2.Delete();
            Assert.That(testDirectory2.Exists, Is.False);
        }

        [Test]
        public void Can_manage_files()
        {
            var file = _fileSystem.GetFile("test.txt");
            Assert.That(file.Exists, Is.False);
            file.Open(FileMode.OpenOrCreate, stream =>
            {
                using (var streamWriter = new StreamWriter(stream))
                    streamWriter.Write("Just a test...");
            });
            Assert.That(file.Exists, Is.True);
            file.Open(FileMode.Open, stream =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    Assert.That(streamReader.ReadToEnd(), Is.EqualTo("Just a test..."));
                }
            });
            file.Delete();
            Assert.That(file.Exists, Is.False);
        }

        [SetUp]
        public void Setup()
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "base");
            if(Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, true);
            _fileSystem = new LocalFileSystem(baseDirectory);
        }
    }
}
