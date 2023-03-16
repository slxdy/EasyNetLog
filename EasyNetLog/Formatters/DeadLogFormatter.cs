using System.Linq;

namespace EasyNetLog.Formatters;

/// <summary>
/// Formatter for removing color codes, primarily used for log files.
/// </summary>
public class DeadLogFormatter : LogFormatter
{
    private static readonly string[] blacklistedSettings = new string[]
    {
        "color"
    };

    protected override string? CloseSetting(string setting)
    {
        if (blacklistedSettings.Contains(setting))
            return string.Empty;

        return null;
    }

    protected override string? OpenSetting(string setting, string? argument)
    {
        if (blacklistedSettings.Contains(setting))
            return string.Empty;

        return null;
    }
}
