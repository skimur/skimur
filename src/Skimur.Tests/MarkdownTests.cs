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
            Assert.That(_markdownCompiler.Compile("*TEST*"), Is.EqualTo("<p><em>TEST</em></p>"));

            var sb = new StringBuilder();
            sb.AppendLine("````");
            sb.AppendLine("test");
            sb.AppendLine("   ttt");
            sb.AppendLine("````");
            Assert.That(_markdownCompiler.Compile(sb.ToString()), Is.EqualTo("<p><em>TEST</em></p>"));
        }

//        ``` js
//var foo = function (bar) {
//  return bar++;
//};

//console.log(foo(5));
//```

        protected override void Setup()
        {
            base.Setup();
            _markdownCompiler = new MarkdownCompiler(new V8JsEngine());
        }
    }
}
