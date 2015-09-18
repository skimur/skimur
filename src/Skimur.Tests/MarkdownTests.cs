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
            Assert.That(_markdownCompiler.Compile("/u/test"), Is.EqualTo("<a href=\"/u/test\">test</a>"));
        }

        protected override void Setup()
        {
            base.Setup();
            _markdownCompiler = new MarkdownCompiler(new V8JsEngine());
        }
    }
}
