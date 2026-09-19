
using System;

namespace TeaSpoons.NumberFormatting
{
    /// <summary>
    /// Provides some
    /// </summary>
    public static class NumberSuffixProviders
    {
        /// <summary>
        /// Returns K, M, B, T and then continues with aa, ab, .. az, ba, bb, .. zz, Aa, Ab up until Zz.
        /// </summary>
        /// <remarks>
        /// Offers 1356 suffixes, which means support for numbers up to 1e4068.
        /// </remarks>
        public static string TrillionThenDoubleLetter(ushort index)
        {
            if (index > 1355) throw new ArgumentOutOfRangeException(nameof(index));

            switch (index)
            {
                case 0: return "K";
                case 1: return "M";
                case 2: return "B";
                case 3: return "T";
                default:
                    index -= 4;

                    ushort lowercaseFirstLetterLimit = 26 * 26;
                    char firstLetter;
                    if (index < 26 * 26)
                    {
                        firstLetter = 'a';
                    }
                    else
                    {
                        firstLetter = 'A';
                        index -= lowercaseFirstLetterLimit;
                    }
                    firstLetter = (char)(firstLetter + (index / 26));

                    var secondLetter = (char)('a' + (index % 26));

                    return string.Concat(firstLetter, secondLetter);
            }
        }
    }
}
