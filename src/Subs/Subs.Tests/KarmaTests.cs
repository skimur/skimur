using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Skimur.Tests;
using Subs.Services;

namespace Subs.Tests
{
    public class KarmaTests : DataTestBase
    {
        private IKarmaService _karmaService;

        [Test]
        public void Can_increase_and_decrease_karma()
        {
            TestWithUser(Guid.NewGuid());
            TestWithUser(Guid.NewGuid());
        }

        [Test]
        public void Can_delete_karma_for_user()
        {
            // arrange
            var userId = Guid.NewGuid();
            var subId = Guid.NewGuid();
            _karmaService.IncreaseKarma(userId, subId, KarmaType.Comment);
            var report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(1));
            Assert.That(report[new KarmaReportKey(subId, KarmaType.Comment)], Is.EqualTo(1));

            // act
            _karmaService.DeleteAllKarmaForUser(userId);

            // assert
            report = _karmaService.GetKarma(userId);
            // it doesn't really delete, just resets
            Assert.That(report.Count, Is.EqualTo(1));
            Assert.That(report[new KarmaReportKey(subId, KarmaType.Comment)], Is.EqualTo(0));

            // act
            _karmaService.IncreaseKarma(userId, subId, KarmaType.Comment);

            // asert
            report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(1));
            Assert.That(report[new KarmaReportKey(subId, KarmaType.Comment)], Is.EqualTo(1));
        }

        private void TestWithUser(Guid userId)
        {
            var sub1 = Guid.NewGuid();
            var sub2 = Guid.NewGuid();

            var report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(0));

            _karmaService.IncreaseKarma(userId, sub1, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(1));
            Assert.That(report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)), Is.True);
            Assert.That(report[new KarmaReportKey(sub1, KarmaType.Comment)], Is.EqualTo(1));

            _karmaService.IncreaseKarma(userId, sub1, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(1));
            Assert.That(report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)), Is.True);
            Assert.That(report[new KarmaReportKey(sub1, KarmaType.Comment)], Is.EqualTo(2));

            _karmaService.IncreaseKarma(userId, sub2, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(2));
            Assert.That(report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)), Is.True);
            Assert.That(report[new KarmaReportKey(sub1, KarmaType.Comment)], Is.EqualTo(2));
            Assert.That(report[new KarmaReportKey(sub2, KarmaType.Comment)], Is.EqualTo(1));

            _karmaService.IncreaseKarma(userId, sub2, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            Assert.That(report.Count, Is.EqualTo(2));
            Assert.That(report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)), Is.True);
            Assert.That(report[new KarmaReportKey(sub1, KarmaType.Comment)], Is.EqualTo(2));
            Assert.That(report[new KarmaReportKey(sub2, KarmaType.Comment)], Is.EqualTo(2));
        }

        protected override void Setup()
        {
            base.Setup();
            _karmaService = _container.GetInstance<IKarmaService>();
        }
    }
}
