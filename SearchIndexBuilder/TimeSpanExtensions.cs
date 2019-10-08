using System;

namespace SearchIndexBuilder
{

    public static class TimeSpanExtensions
    {
        public static string FormatForDisplay(this TimeSpan ts, bool includeMs = false)
        {
            string result = string.Empty;

            if (ts.Hours > 0)
            {
                result += ts.Hours + "h";
            }

            if (ts.Minutes > 0)
            {
                result += ts.Minutes + "m";
            }

            if (ts.Seconds > 0)
            {
                result += ts.Seconds + "s";
            }

            if (includeMs || result.Length == 0)
            {
                if (ts.Milliseconds > 0 || result.Length == 0)
                {
                    result += ts.Milliseconds + "ms";
                }
            }

            return result;
        }
    }

}
