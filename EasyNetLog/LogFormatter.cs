using System.Collections.Generic;
using System.Text;

namespace EasyNetLog
{
    public abstract class LogFormatter
    {
        public string Format(string log)
        {
            var openRead = false;
            var lastCmdCharIdx = 0;
            var settings = new List<string>();
            var replacements = new List<Replacement>();
            var ignore = false;

            for (var idx = 0; idx < log.Length; idx++)
            {
                var c = log[idx];

                if (c == '<')
                {
                    lastCmdCharIdx = idx;
                    openRead = true;
                    continue;
                }

                if (openRead)
                {
                    if (c == '>')
                    {
                        openRead = false;
                        var isClosing = log[lastCmdCharIdx + 1] == '/';
                        var setting = log.Substring(lastCmdCharIdx + (isClosing ? 2 : 1), idx - lastCmdCharIdx - (isClosing ? 2 : 1));
                        string? argument = null;
                        var splitIdx = setting.IndexOf('=');
                        if (splitIdx != -1)
                        {
                            argument = setting.Substring(splitIdx + 1);
                            setting = setting.Substring(0, splitIdx).ToLower();
                        }
                        var isIgnore = setting == "ignore";
                        if (ignore && !isIgnore)
                            continue;

                        if (isClosing)
                        {
                            var settingCounterIdx = settings.IndexOf(setting);
                            if (settingCounterIdx == -1)
                                continue;

                            settings.RemoveAt(settingCounterIdx);

                            if (isIgnore)
                            {
                                if (ignore)
                                {
                                    replacements.Add(new Replacement(string.Empty, lastCmdCharIdx, idx - lastCmdCharIdx + 1));
                                    ignore = false;
                                }

                                continue;
                            }

                            var closeReplacement = CloseSetting(setting);
                            if (closeReplacement != null)
                            {
                                replacements.Add(new Replacement(closeReplacement, lastCmdCharIdx, idx - lastCmdCharIdx + 1));
                            }
                            continue;
                        }

                        settings.Add(setting);

                        if (isIgnore)
                        {
                            if (!ignore)
                            {
                                replacements.Add(new Replacement(string.Empty, lastCmdCharIdx, idx - lastCmdCharIdx + 1));
                                ignore = true;
                            }

                            continue;
                        }

                        var openReplacement = OpenSetting(setting, argument);
                        if (openReplacement != null)
                        {
                            replacements.Add(new Replacement(openReplacement, lastCmdCharIdx, idx - lastCmdCharIdx + 1));
                        }
                    }
                }
            }

            var sb = new StringBuilder();
            var nextIdx = 0;
            foreach (var replacement in replacements)
            {
                if (replacement.replacement == null)
                    continue;

                var appendLength = replacement.startIdx - nextIdx;
                sb.Append(log.Substring(nextIdx, appendLength));
                sb.Append(replacement.replacement);
                nextIdx += appendLength + replacement.length;
            }

            if (nextIdx < log.Length)
                sb.Append(log.Substring(nextIdx, log.Length - nextIdx));

            foreach (var setting in settings)
            {
                var closing = CloseSetting(setting);
                if (closing == null)
                    continue;

                sb.Append(closing);
            }

            return sb.ToString();
        }

        protected abstract string? OpenSetting(string setting, string? argument);
        protected abstract string? CloseSetting(string setting);

        private class Replacement
        {
            public string replacement;
            public int startIdx;
            public int length;

            public Replacement(string replacement, int startIdx, int length)
            {
                this.replacement = replacement;
                this.startIdx = startIdx;
                this.length = length;
            }
        }
    }
}
