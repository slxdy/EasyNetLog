using EasyNetLog.Formatters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace EasyNetLog
{
    public class EasyNetLogger
    {
        private readonly LogFormat logFormat;
        private readonly List<LogStream> _logStreams = new List<LogStream>();

        public ReadOnlyCollection<LogStream> LogStreams => _logStreams.AsReadOnly();

        public EasyNetLogger(LogFormat logFormat, bool includeConsoleStream, IEnumerable<string> files, IEnumerable<LogStream> customLogStreams)
        {
            this.logFormat = logFormat;

            if (includeConsoleStream)
                _logStreams.Add(new LogStream(Console.Out, new ConsoleLogFormatter()));

            if (files != null)
            {
                foreach (var file in files)
                {
                    try
                    {
                        var dir = Path.GetDirectoryName(file);
                        if (dir == null)
                            continue;

                        Directory.CreateDirectory(dir);

                        var str = File.CreateText(file);
                        _logStreams.Add(new LogStream(str, new DeadLogFormatter()));
                    }
                    catch
                    {

                    }
                }
            }

            if (customLogStreams != null)
            {
                _logStreams.AddRange(customLogStreams);
            }
        }

        public void Log(string log)
        {
            var finalLog = logFormat == null ? log : logFormat.Invoke(log);

            foreach (var logStream in _logStreams)
            {
                var finalFinalLog = logStream.formatter.Format(finalLog);
                logStream.stream.WriteLine(finalFinalLog);
                logStream.stream.Flush();
            }
        }

        public void NewLine()
        {
            foreach (var logStream in _logStreams)
            {
                logStream.stream.WriteLine();
                logStream.stream.Flush();
            }
        }
    }
}
