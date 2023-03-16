using System;
using System.ComponentModel;
using System.Drawing;

namespace EasyNetLog.Utilities;

public static class ColorTranslator
{
    public static Color FromHtml(string htmlColor)
    {
        Color c = Color.Empty;

        // empty color
        if ((htmlColor == null) || (htmlColor.Length == 0))
            return c;

        // #RRGGBB or #RGB 
        if ((htmlColor[0] == '#') &&
            ((htmlColor.Length == 7) || (htmlColor.Length == 4)))
        {

            if (htmlColor.Length == 7)
            {
                c = Color.FromArgb(Convert.ToInt32(htmlColor.Substring(1, 2), 16),
                                   Convert.ToInt32(htmlColor.Substring(3, 2), 16),
                                   Convert.ToInt32(htmlColor.Substring(5, 2), 16));
            }
            else
            {
                string r = Char.ToString(htmlColor[1]);
                string g = Char.ToString(htmlColor[2]);
                string b = Char.ToString(htmlColor[3]);

                c = Color.FromArgb(Convert.ToInt32(r + r, 16),
                                   Convert.ToInt32(g + g, 16),
                                   Convert.ToInt32(b + b, 16));
            }
        }

        // special case. Html requires LightGrey, but .NET uses LightGray 
        if (c.IsEmpty && String.Equals(htmlColor, "LightGrey", StringComparison.OrdinalIgnoreCase))
        {
            c = Color.LightGray;
        }

        // System color 
        if (c.IsEmpty)
        {
            c = Color.FromName(htmlColor);
        }

        // resort to type converter which will handle named colors 
        if (c.IsEmpty)
        {
            c = (Color)TypeDescriptor.GetConverter(typeof(Color)).ConvertFromString(htmlColor);
        }

        return c;
    }
}
