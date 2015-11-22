using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Netric.Shared;

namespace Netric.EventConsumer.Flamegraphs
{
    /// <summary>
    /// Where to save the flamegraph SVG?
    /// </summary>
    public class MarkupWriter
    {
        private readonly string _directoryPath;
        private static readonly HashSet<char> InvalidChars = new HashSet<char>(Path.GetInvalidFileNameChars());

        public MarkupWriter()
        {
            _directoryPath = ResolveDirectoryPath();            
        }

        public TextWriter GetWriter(Request request)
        {
            var fileName = ResolveFileName(request);
            return new StreamWriter(Path.Combine(_directoryPath, fileName));
        }

        private string ResolveFileName(Request request)
        {
            var name = new string(request.Url.Select(ch => InvalidChars.Contains(ch) ? '_' : ch).ToArray());
            var date = request.StartTime.ToString("yyMMddhhmmss");

            return string.Format("req_{0}_{1}_{2}.svg", name, date, request.ElapsedTicks/10000);
        }

        private string ResolveDirectoryPath()
        {
            var dirPath = Path.Combine(Environment.CurrentDirectory, "flames");
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            return dirPath;
        }
    }
}