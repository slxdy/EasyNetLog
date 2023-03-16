namespace EasyNetLog;

/// <summary>
/// A delegate used for log preprocessors.
/// </summary>
/// <param name="log">The message of the log.</param>
/// <returns>The final log.</returns>
public delegate string LogFormat(string log);