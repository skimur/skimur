using Microsoft.AspNet.NodeServices;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Threading;

namespace Skimur.Markdown
{
    public class MarkdownCompiler : IMarkdownCompiler
    {
        TempScriptDirectory _scriptsDirectory;
        INodeServices _nodeServices;

        public MarkdownCompiler(IPathResolver pathResolver)
        {
            _scriptsDirectory = new TempScriptDirectory();
            _nodeServices = Configuration.CreateNodeServices(NodeHostingModel.Http, _scriptsDirectory.TempDirectory);
        }
        
        public string Compile(string markdown)
        {
            if (string.IsNullOrEmpty(markdown)) return null;
            var invoke = _nodeServices.InvokeExport<MarkdownCompileResult>(_scriptsDirectory.CompileScriptPath, "compileMarkdown", markdown);
            invoke.Wait();
            return invoke.Result.Result;
        }

        public string Compile(string markdown, out List<string> mentions)
        {
            mentions = new List<string>();

            if (string.IsNullOrEmpty(markdown)) return null;

            var invoke = _nodeServices.InvokeExport<MarkdownCompileResult>(_scriptsDirectory.CompileScriptPath, "compileMarkdown", markdown);
            invoke.Wait();

            if (invoke.Result.Mentions != null && invoke.Result.Mentions.Count > 0)
                mentions.AddRange(invoke.Result.Mentions);

            return invoke.Result.Result;
        }

        public void Dispose()
        {
            if (_nodeServices != null)
            {
                _nodeServices.Dispose();
                _nodeServices = null;
                // the node services doesn't "WaitForExit". it should though.
                // this should be enough time for the process to fully exit.
                Thread.Sleep(TimeSpan.FromSeconds(.5));
            }

            if(_scriptsDirectory != null)
            {
                _scriptsDirectory.Dispose();
                _scriptsDirectory = null;
            }
        }

        class MarkdownCompileResult
        {
            public string Result { get; set; }

            public List<string> Mentions { get; set; }
        }

        public sealed class TempScriptDirectory : IDisposable
        {
            public string TempDirectory { get; private set; }

            public string CompileScriptPath { get; private set; }

            private bool _disposedValue;

            public TempScriptDirectory()
            {
                var directory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));
                while (Directory.Exists(directory))
                    directory = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName()));

                Directory.CreateDirectory(directory);

                File.WriteAllText(Path.Combine(directory, "marked.js"),
                    EmbeddedResourceReader.Read(typeof(TempScriptDirectory), "/Content/Node/marked.js"));
                File.WriteAllText(Path.Combine(directory, "markedHelper.js"),
                    EmbeddedResourceReader.Read(typeof(TempScriptDirectory), "/Content/Node/markedHelper.js"));
                File.WriteAllText(Path.Combine(directory, "compile-markdown.js"),
                    EmbeddedResourceReader.Read(typeof(TempScriptDirectory), "/Content/Node/compile-markdown.js"));

                TempDirectory = directory;

                CompileScriptPath = Path.Combine(TempDirectory, "compile-markdown.js");
            }

            private void DisposeImpl(bool disposing)
            {
                if (!_disposedValue)
                {
                    Directory.Delete(TempDirectory, true);

                    _disposedValue = true;
                }
            }

            public void Dispose()
            {
                DisposeImpl(true);
                GC.SuppressFinalize(this);
            }

            ~TempScriptDirectory()
            {
                DisposeImpl(false);
            }
        }
    }
}
