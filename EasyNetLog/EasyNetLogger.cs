using EasyNetLog.Formatters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;

namespace EasyNetLog;

/// <summary>
/// The main logger class.
/// </summary>
public class EasyNetLogger
{
    private readonly LogFormat logFormat;
    private readonly List<LogStream> _logStreams = new();

    [DllImport("kernel32")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll")]
    private static extern IntPtr GetConsoleWindow();

    /// <summary>
    /// A collection of all the streams this logger flushes to.
    /// </summary>
    public ReadOnlyCollection<LogStream> LogStreams => _logStreams.AsReadOnly();

    /// <param name="logFormat">A log preprocessor. Example lambda: <i>(msg) => $"[&lt;color=red&gt;Cool Log&lt;/color&gt;] {msg}"</i></param>
    /// <param name="includeConsoleStream">Logs to console if true. A console is allocated if no other console is attached.</param>
    /// <param name="files">A collection of file paths that the logger will log to.</param>
    /// <param name="streams">A collection of streams that the logger will log to.</param>
    public EasyNetLogger(LogFormat logFormat, bool includeConsoleStream, IEnumerable<string>? files = null, IEnumerable<LogStream>? streams = null)
    {
        this.logFormat = logFormat;

        if (includeConsoleStream)
        {
            var hasConsole = GetConsoleWindow() != IntPtr.Zero;
            if (!hasConsole)
            {
                hasConsole = AllocConsole();
                if (hasConsole)
                    Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
            }
            if (hasConsole)
            {
                _logStreams.Add(new LogStream(Console.Out, new ConsoleLogFormatter()));
            }
        }

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

        if (streams != null)
        {
            _logStreams.AddRange(streams);
        }
    }

    /// <summary>
    /// Logs a message to all <see cref="LogStreams"/>. See <see href="https://github.com/MikeTheRealNerd/EasyNetLog#formatting">README</see> for more info on how formatting works.
    /// </summary>
    public void Log(string log)
    {
        var finalLog = logFormat == null ? log : logFormat.Invoke(log);

        foreach (var logStream in _logStreams)
        {
            var finalFinalLog = logStream.Formatter.Format(finalLog);
            logStream.Stream.WriteLine(finalFinalLog);
            logStream.Stream.Flush();
        }
    }

    /// <summary>
    /// Logs a new line to all <see cref="LogStreams"/>.
    /// </summary>
    public void NewLine()
    {
        foreach (var logStream in _logStreams)
        {
            logStream.Stream.WriteLine();
            logStream.Stream.Flush();
        }
    }
}
