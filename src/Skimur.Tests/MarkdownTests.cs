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

        [Test, Ignore]
        public void Can_render_user_as_link()
        {
            Assert.That(_markdownCompiler.Compile("/u/test"), Is.EqualTo("<p><a href=\"/user/test\" class=\"user-mention\">/u/test</a></p>\n"));
            Assert.That(_markdownCompiler.Compile("@test"), Is.EqualTo("<p><a href=\"/user/test\" class=\"user-mention\">@test</a></p>\n"));
            Assert.That(_markdownCompiler.Compile("Hi @test, whats up?"), Is.EqualTo("<p>Hi <a href=\"/user/test\" class=\"user-mention\">@test</a>, whats up?</p>\n"));
        }

        protected override void Setup()
        {
            base.Setup();
            _markdownCompiler = new MarkdownCompiler(new V8JsEngine());
        }
    }
}
