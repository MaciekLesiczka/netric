using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Netric.Intercept.Web
{
    public class PreBodyTagFilter : Stream
    {
        private const string BodyClosingTag = "</body>";        

        private string HtmlSnippet { get; set; }

        private Stream OutputStream { get; set; }

        private Encoding ContentEncoding { get; set; }

        private Regex BodyEndRegex { get; set; }

        private string CurrentRequestRawUrl { get; set; }

        private string UnwrittenCharactersFromPreviousCall { get; set; }

        public PreBodyTagFilter(string htmlSnippet, Stream outputStream, Encoding contentEncoding, string currentRequestRawUrl
            )
        {
            HtmlSnippet = htmlSnippet + BodyClosingTag;
            OutputStream = outputStream;
            ContentEncoding = contentEncoding;
            BodyEndRegex = new Regex(BodyClosingTag, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.RightToLeft);
            CurrentRequestRawUrl = currentRequestRawUrl ?? "unknown";            
        }

        public override bool CanRead
        {
            get { return OutputStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return OutputStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return OutputStream.CanWrite; }
        }

        public override long Length
        {
            get { return OutputStream.Length; }
        }

        public override long Position
        {
            get { return OutputStream.Position; }
            set { OutputStream.Position = value; }
        }

        public override void Close()
        {
            OutputStream.Close();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return OutputStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            OutputStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return OutputStream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
           
            string contentInBuffer = ContentEncoding.GetString(buffer, offset, count);

           
            if (!string.IsNullOrEmpty(UnwrittenCharactersFromPreviousCall))
            {
                contentInBuffer = UnwrittenCharactersFromPreviousCall + contentInBuffer;
                UnwrittenCharactersFromPreviousCall = null;
            }

            Match closingBodyTagMatch = BodyEndRegex.Match(contentInBuffer);
            if (closingBodyTagMatch.Success)
            {
                WriteToOutputStream(contentInBuffer.Substring(0, closingBodyTagMatch.Index));
                UnwrittenCharactersFromPreviousCall = contentInBuffer.Substring(closingBodyTagMatch.Index);
            }
            else
            {
                if (contentInBuffer.Length <= 10)
                {
                    UnwrittenCharactersFromPreviousCall = contentInBuffer;
                }
                else
                {
                    WriteToOutputStream(contentInBuffer.Substring(0, contentInBuffer.Length - 10));
                    UnwrittenCharactersFromPreviousCall = contentInBuffer.Substring(contentInBuffer.Length - 10);
                }
            }
        }

        public override void Flush()
        {
            if (!string.IsNullOrEmpty(UnwrittenCharactersFromPreviousCall))
            {
                string finalContentToWrite = UnwrittenCharactersFromPreviousCall;

                if (BodyEndRegex.IsMatch(UnwrittenCharactersFromPreviousCall))
                {                    
                    finalContentToWrite = BodyEndRegex.Replace(UnwrittenCharactersFromPreviousCall, HtmlSnippet, 1);
                }
                WriteToOutputStream(finalContentToWrite);
            }

            OutputStream.Flush();
        }

        private void WriteToOutputStream(string content)
        {
            byte[] outputBuffer = ContentEncoding.GetBytes(content);
            OutputStream.Write(outputBuffer, 0, outputBuffer.Length);
        }
    }
}