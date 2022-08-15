using System.IO;

namespace EasyNetLog
{
    public class LogStream
    {
        public readonly TextWriter stream;
        public readonly LogFormatter formatter;

        public LogStream(TextWriter stream, LogFormatter formatter)
        {
            this.stream = stream;
            this.formatter = formatter;
        }
    }
}
