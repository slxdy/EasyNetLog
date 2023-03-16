using EasyNetLog.Utilities;
using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EasyNetLog.Formatters
{
    public class ConsoleLogFormatter : LogFormatter
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private static readonly int[] consoleColors =
        {
            0x000000, //Black = 0
            0x000080, //DarkBlue = 1
            0x008000, //DarkGreen = 2
            0x008080, //DarkCyan = 3
            0x800000, //DarkRed = 4
            0x800080, //DarkMagenta = 5
            0x808000, //DarkYellow = 6
            0xC0C0C0, //Gray = 7
            0x808080, //DarkGray = 8
            0x0000FF, //Blue = 9
            0x00FF00, //Green = 10
            0x00FFFF, //Cyan = 11
            0xFF0000, //Red = 12
            0xFF00FF, //Magenta = 13
            0xFFFF00, //Yellow = 14
            0xFFFFFF  //White = 15
        };

        private static readonly bool colorProcessingEnabled;
        private TextColor? currentColor;

        static ConsoleLogFormatter()
        {
            if (Environment.GetEnvironmentVariable("NO_COLOR") != null)
                return;

            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);

            colorProcessingEnabled = GetConsoleMode(iStdOut, out var outConsoleMode) &&
                                        SetConsoleMode(iStdOut, outConsoleMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        private static Color GetConsoleColor(ConsoleColor color)
        {
            return Color.FromArgb(consoleColors[(int)color]);
        }

        protected override string? CloseSetting(string setting)
        {
            switch (setting)
            {
                case "color":
                    return CloseColor();
            }

            return null;
        }

        protected override string? OpenSetting(string setting, string? argument)
        {
            switch (setting)
            {
                case "color":
                    return OpenColor(argument);
            }

            return null;
        }

        private string CloseColor()
        {
            if (!colorProcessingEnabled)
                return string.Empty;

            if (currentColor != null)
                currentColor = currentColor.parent;

            var prevColor = currentColor == null ? GetConsoleColor(Console.ForegroundColor) : currentColor.color;
            return GetOpenColorFormat(prevColor);
        }

        private string OpenColor(string? argument)
        {
            if (!colorProcessingEnabled)
                return string.Empty;

            Color color;
            if (argument == null)
            {
                color = GetConsoleColor(Console.ForegroundColor);
            }
            else
            {
                color = ColorTranslator.FromHtml(argument);
            }

            currentColor = new TextColor
            {
                parent = currentColor,
                color = color
            };
            return GetOpenColorFormat(color);
        }

        private string GetOpenColorFormat(Color color)
        {
            return $"\u001b[38;2;{color.R};{color.G};{color.B}m";
        }

        private class TextColor
        {
            public TextColor? parent;
            public Color color;
        }
    }
}
