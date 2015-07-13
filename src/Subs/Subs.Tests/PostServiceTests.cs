using System.Collections.Generic;
using NUnit.Framework;
using Skimur;
using Skimur.Tests;

namespace Subs.Tests
{
    [TestFixture]
    public class PostServiceTests : DataTestBase
    {
        [Test]
        public void Can_get_posts()
        {
            
        }

        protected override List<IRegistrar> GetRegistrars()
        {
            var result = base.GetRegistrars();

            // ReSharper disable RedundantNameQualifier
            result.Add(new Subs.Registrar());
            // ReSharper restore RedundantNameQualifier

            return result;
        }
    }
}
