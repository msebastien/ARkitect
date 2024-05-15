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
    }

}
