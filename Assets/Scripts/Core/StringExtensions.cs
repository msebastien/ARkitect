using System;

namespace ARKitect.Core
{
    internal static class StringExtensions
    {
        public static int CountOccurrences(this string self, string str)
        {
            int count = 0;
            int startIndex = 0;

            int currIndex;
            while ((currIndex = self.IndexOf(str, startIndex, StringComparison.CurrentCultureIgnoreCase)) != -1)
            {
                count++;
                startIndex = currIndex + str.Length;
            }

            return count;
        }

        public static TEnum ToEnum<TEnum>(this string self, TEnum defaultValue) where TEnum : struct
        {
            if (string.IsNullOrEmpty(self))
            {
                return defaultValue;
            }

            return Enum.TryParse<TEnum>(self, true, out TEnum result) ? result : defaultValue;
        }
    }

}
