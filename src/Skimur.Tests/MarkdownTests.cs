using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JavaScriptEngineSwitcher.V8;
using NUnit.Framework;
using Skimur.Markdown;

namespace Skimur.Tests
{
    [TestFixture]
    public class MarkdownTests : TestBase
    {
        IMarkdownCompiler _markdownCompiler;

        [Test]
        public void Can_render_markdown()
        {
            Assert.That(_markdownCompiler.Compile("*TEST*"), Is.EqualTo("<p><em>TEST</em></p>\n"));
        }

        [Test]
        public void Can_render_user_as_link()
        {
            List<string> mentions;
            Assert.That(_markdownCompiler.Compile("/u/test", out mentions), Is.EqualTo("<p><a href=\"/user/test\" class=\"user-mention\">/u/test</a></p>\n"));
            Assert.That(mentions, Has.Count.EqualTo(1));
            Assert.That(mentions[0], Is.EqualTo("test"));
            Assert.That(_markdownCompiler.Compile("@test", out mentions), Is.EqualTo("<p><a href=\"/user/test\" class=\"user-mention\">@test</a></p>\n"));
            Assert.That(mentions, Has.Count.EqualTo(1));
            Assert.That(mentions[0], Is.EqualTo("test"));
            Assert.That(_markdownCompiler.Compile("Hi @test, whats up?", out mentions), Is.EqualTo("<p>Hi <a href=\"/user/test\" class=\"user-mention\">@test</a>, whats up?</p>\n"));
            Assert.That(mentions, Has.Count.EqualTo(1));
            Assert.That(mentions[0], Is.EqualTo("test"));
        }

        protected override void Setup()
        {
            base.Setup();
            _markdownCompiler = new MarkdownCompiler(new V8JsEngine());
        }
    }
}
