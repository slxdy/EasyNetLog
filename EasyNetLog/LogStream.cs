using System.IO;

namespace EasyNetLog;

public class LogStream
{
    public TextWriter Stream { get; private set; }
    public LogFormatter Formatter { get; private set; }

    public LogStream(TextWriter stream, LogFormatter formatter)
    {
        Stream = stream;
        Formatter = formatter;
    }
}
