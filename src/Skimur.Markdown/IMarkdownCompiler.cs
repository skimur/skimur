using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skimur.Markdown
{
    public interface IMarkdownCompiler : IDisposable
    {
        string Compile(string markdown);

        string Compile(string markdown, out List<string> mentions);
    }
}
