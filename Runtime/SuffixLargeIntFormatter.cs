
#if LARGE_NUMBERS
namespace TeaSpoons.NumberFormatting
{
    using TeaSpoons.LargeNumbers;
    using System;
    using System.Text;
    using UnityEngine;

    /// <inheritdoc/>
    /// <summary>
    /// An <see cref="INumberFormatter{LargeInt}"/> that shortens numbers using suffixes, like 1.10k instead of 1100.
    /// For all numbers >= 100, there are always 3 relevant digits.
    /// </summary>
    public class SuffixLargeIntFormatter : INumberFormatter<LargeInt>
    {
        private static readonly StringBuilder stringBuilder = new StringBuilder();

        private const char defaultDecimalSeperator = '.';

        private readonly char decimalSeparator;
        private readonly SuffixProvider suffixProvider;
        private readonly int plainNumberLimit;
        private readonly byte visibleDigitCount;

        /// <summary>
        /// Creates a new <see cref="SuffixLongFormatter"/>.
        /// </summary>
        /// <param name="suffixes">The list of suffixes, with the first one being for 10^3 (e.g. "k" for "10k"), the second one for 10^6 (e.g. "m" for "10m") and so on.</param>
        /// <param name="decimalSeparator">The separator used for shortened numbers (e.g. "1.2k").</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="suffixes"/> is <c>null</c>.</exception>
        public SuffixLargeIntFormatter(SuffixProvider suffixProvider,
            char decimalSeparator = defaultDecimalSeperator,
            int plainNumberLimit = 1_000,
            byte visibleDigitCount = 3)
        {
            this.suffixProvider = suffixProvider ?? throw new ArgumentNullException(nameof(suffixProvider));
            this.decimalSeparator = decimalSeparator;
            this.plainNumberLimit = plainNumberLimit;
            this.visibleDigitCount = visibleDigitCount;
        }

        public string FormatAmount(LargeInt amount)
        {
            return ComposeString(amount, true);
        }

        public string FormatCost(LargeInt cost)
        {
            return ComposeString(cost, false);
        }

        private string ComposeString(LargeInt amount, bool floorWhenPositive)
        {
            var sign = amount >= 0 ? 1 : -1;
            var negative = sign == -1;
            var absolute = negative ? -amount : amount;

            if (absolute < plainNumberLimit)
            {
                return Mathf.RoundToInt((float)amount.FloatingDigits * Pow10(amount.Exponent)).ToString();
            }

            stringBuilder.Clear();

            // The factor by which we will multiply the digits in order to have the desired amount of visible digits
            var visibleDigitsFactor = Pow10((ushort)(visibleDigitCount - 1));

            // If the number is supposed to be ceiled rather than floored, we add a number consisting of one 9
            // for each digit we pushed to the left. This turns the flooring operation into ceiling.
            var shouldCeil = negative == floorWhenPositive;
            var digits = absolute.RawDigits * visibleDigitsFactor;
            var remainder = digits % LargeInt.ScaleFactor;
            digits /= LargeInt.ScaleFactor;

            if (shouldCeil && remainder > 0)
            {
                digits += 1;
            }

            // Calculate the amount of digits we want to display before the decimal separator.
            var digitsBeforeDecimalCount = (byte)((absolute.Exponent % 3) + 1);

            if (negative)
            {
                stringBuilder.Append('-');
            }

            var suffixIndex = absolute.Exponent / 3 - 1;
            string suffix;
            try
            {
                suffix = suffixProvider((ushort)suffixIndex);
            }
            catch (ArgumentOutOfRangeException)
            {
                // Append infinity sign
                stringBuilder.Append('\u221e');
                return stringBuilder.ToString();
            }

            // Append the visible digits with the decimal separator at the correct position.
            var digitsString = digits.ToString();
            stringBuilder.Append(digitsString, 0, digitsBeforeDecimalCount);

            var fractionalDigitCount = visibleDigitCount - digitsBeforeDecimalCount;
            if (fractionalDigitCount > 0)
            {
                // Trim trailing zeros from the fractional part
                var fractionalPart = digitsString.Substring(digitsBeforeDecimalCount, fractionalDigitCount);
                fractionalPart = fractionalPart.TrimEnd('0');
                
                if (fractionalPart.Length > 0)
                {
                    stringBuilder.Append(decimalSeparator);
                    stringBuilder.Append(fractionalPart);
                }
            }

            if (suffixIndex >= 0)
            {
                stringBuilder.Append(suffix);
            }

            return stringBuilder.ToString();
        }

        private static uint Pow10(ushort exponent)
        {
            uint result = 1;
            while (exponent > 0)
            {
                result *= 10;
                exponent--;
            }
            return result;
        }
    }
}
#endif
