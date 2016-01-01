using System;
using FluentAssertions;
using Skimur.App.Services;
using Skimur.Tests;
using Xunit;
using Microsoft.Extensions.DependencyInjection;

namespace Skimur.App.Tests
{
    public class KarmaTests : DataTestBase
    {
        private IKarmaService _karmaService;

        public KarmaTests()
        {
            _karmaService = _serviceProvider.GetService<IKarmaService>();
        }

        [Fact]
        public void Can_increase_and_decrease_karma()
        {
            TestWithUser(Guid.NewGuid());
            TestWithUser(Guid.NewGuid());
        }

        [Fact]
        public void Can_delete_karma_for_user()
        {
            // arrange
            var userId = Guid.NewGuid();
            var subId = Guid.NewGuid();
            _karmaService.IncreaseKarma(userId, subId, KarmaType.Comment);
            var report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(1);
            report[new KarmaReportKey(subId, KarmaType.Comment)].Should().Be(1);

            // act
            _karmaService.DeleteAllKarmaForUser(userId);

            // assert
            report = _karmaService.GetKarma(userId);
            // it doesn't really delete, just resets
            report.Count.Should().Be(1);
            report[new KarmaReportKey(subId, KarmaType.Comment)].Should().Be(0);

            // act
            _karmaService.IncreaseKarma(userId, subId, KarmaType.Comment);

            // asert
            report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(1);
            report[new KarmaReportKey(subId, KarmaType.Comment)].Should().Be(1);
        }

        private void TestWithUser(Guid userId)
        {
            var sub1 = Guid.NewGuid();
            var sub2 = Guid.NewGuid();

            var report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(0);

            _karmaService.IncreaseKarma(userId, sub1, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(1);
            report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)).Should().BeTrue();
            report[new KarmaReportKey(sub1, KarmaType.Comment)].Should().Be(1);

            _karmaService.IncreaseKarma(userId, sub1, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(1);
            report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)).Should().BeTrue();
            report[new KarmaReportKey(sub1, KarmaType.Comment)].Should().Be(2);

            _karmaService.IncreaseKarma(userId, sub2, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(2);
            report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)).Should().BeTrue();
            report[new KarmaReportKey(sub1, KarmaType.Comment)].Should().Be(2);
            report[new KarmaReportKey(sub2, KarmaType.Comment)].Should().Be(1);

            _karmaService.IncreaseKarma(userId, sub2, KarmaType.Comment);

            report = _karmaService.GetKarma(userId);
            report.Count.Should().Be(2);
            report.ContainsKey(new KarmaReportKey(sub1, KarmaType.Comment)).Should().BeTrue();
            report[new KarmaReportKey(sub1, KarmaType.Comment)].Should().Be(2);
            report[new KarmaReportKey(sub2, KarmaType.Comment)].Should().Be(2);
        }
    }
}
