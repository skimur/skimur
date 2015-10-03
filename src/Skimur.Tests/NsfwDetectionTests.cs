using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Skimur.Tests
{
    [TestFixture]
    public class NsfwDetectionTests
    {
        [Test]
        public void Can_detect_nsfw()
        {
            Assert.That(Common.IsNsfw("Test"), Is.False);
            Assert.That(Common.IsNsfw("NSFW test"), Is.True);
            Assert.That(Common.IsNsfw("NSFW: test"), Is.True);
            Assert.That(Common.IsNsfw("NSFL test"), Is.True);
            Assert.That(Common.IsNsfw("NSFL: test"), Is.True);
        }
    }
}
