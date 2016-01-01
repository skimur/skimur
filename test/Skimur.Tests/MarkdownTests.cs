using System;
using System.Collections.Generic;
using FluentAssertions;
using Skimur.Markdown;
using Xunit;

namespace Skimur.Tests
{
    public class MarkdownTests : IDisposable
    {
        IMarkdownCompiler _markdownCompiler;

        public MarkdownTests()
        {
            _markdownCompiler = new MarkdownCompiler(new PathResolver());
        }

        [Fact]
        public void Can_render_markdown()
        {
            _markdownCompiler.Compile("*TEST*").Should().BeEquivalentTo("<p><em>TEST</em></p>\n");
        }

        [Fact]
        public void Can_render_user_as_link()
        {
            List<string> mentions;
            _markdownCompiler.Compile("/u/test", out mentions)
                .Should()
                .BeEquivalentTo("<p><a href=\"/user/test\" class=\"user-mention\">/u/test</a></p>\n");
            mentions.Count.Should().Be(1);
            mentions[0].Should().Be("test");
            _markdownCompiler.Compile("@test", out mentions)
                .Should()
                .BeEquivalentTo("<p><a href=\"/user/test\" class=\"user-mention\">@test</a></p>\n");
            mentions.Count.Should().Be(1);
            mentions[0].Should().Be("test");
            _markdownCompiler.Compile("Hi @test, whats up?", out mentions)
                .Should()
                .BeEquivalentTo("<p>Hi <a href=\"/user/test\" class=\"user-mention\">@test</a>, whats up?</p>\n");
            mentions.Count.Should().Be(1);
            mentions[0].Should().Be("test");
        }

        public void Dispose()
        {
            _markdownCompiler.Dispose();
        }
    }
}
