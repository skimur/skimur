using System;
using System.IO;
using FluentAssertions;
using Skimur.FileSystem;
using Xunit;

namespace Skimur.Tests
{
    public class LocalFileSystemTests
    {
        private IFileSystem _fileSystem;

        public LocalFileSystemTests()
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "base");
            if (Directory.Exists(baseDirectory))
                Directory.Delete(baseDirectory, true);
            _fileSystem = new LocalFileSystem(baseDirectory);
        }

        [Fact]
        public void Can_manage_directories()
        {
            var testDirectory1 = _fileSystem.GetDirectory("test");
            testDirectory1.Exists.Should().BeFalse();
            testDirectory1.Create();
            testDirectory1.Exists.Should().BeTrue();
            testDirectory1.Delete();
            testDirectory1.Exists.Should().BeFalse();

            var testDirectory2 = _fileSystem.GetDirectory("test/directory");
            testDirectory2.Exists.Should().BeFalse();
            testDirectory2.Create();
            testDirectory2.Exists.Should().BeTrue();
            testDirectory2.Delete();
            testDirectory2.Exists.Should().BeFalse();
        }

        [Fact]
        public void Can_manage_files()
        {
            var file = _fileSystem.GetFile("test.txt");
            file.Exists.Should().BeFalse();
            file.Open(FileMode.OpenOrCreate, stream =>
            {
                using (var streamWriter = new StreamWriter(stream))
                    streamWriter.Write("Just a test...");
            });
            file.Exists.Should().BeTrue();
            file.Open(FileMode.Open, stream =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    streamReader.ReadToEnd().Should().BeEquivalentTo("Just a test...");
                }
            });
            file.Delete();
            file.Exists.Should().BeFalse();
        }
    }
}
