
namespace TeaSpoons.NumberFormatting
{
    using System;
    using System.Text;

    /// <inheritdoc/>
    /// <summary>
    /// An <see cref="INumberFormatter{long}"/> that shortens numbers using suffixes, like 1.10k instead of 1100.
    /// </summary>
    public class SuffixLongFormatter : INumberFormatter<long>
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
        public SuffixLongFormatter(SuffixProvider suffixProvider,
            char decimalSeparator = defaultDecimalSeperator,
            int plainNumberLimit = 1_000,
            byte visibleDigitCount = 3)
        {
            this.suffixProvider = suffixProvider ?? throw new ArgumentNullException(nameof(suffixProvider));
            this.decimalSeparator = decimalSeparator;
            this.plainNumberLimit = plainNumberLimit;
            this.visibleDigitCount = visibleDigitCount;
        }

        public string FormatAmount(long amount)
        {
            var negative = amount < 0;
            var absolute = negative ? -amount : amount;

            if (absolute < plainNumberLimit)
            {
                return amount.ToString();
            }

            stringBuilder.Clear();

            if (negative)
            {
                stringBuilder.Append('-');
            }

            var relevantDigits = GetRelevantPositiveDigits(absolute, out var suffixIndex, out var digitsBeforeDecimal, negative);

            ComposeString(relevantDigits, suffixIndex, digitsBeforeDecimal);

            return stringBuilder.ToString();
        }

        public string FormatCost(long cost)
        {
            var negative = cost < 0;
            var absolute = negative ? -cost : cost;

            if (absolute < plainNumberLimit)
            {
                return cost.ToString();
            }

            stringBuilder.Clear();

            if (negative)
            {
                stringBuilder.Append('-');
            }

            var relevantDigits = GetRelevantPositiveDigits(absolute, out var suffixIndex, out var digitsBeforeDecimal, !negative);

            ComposeString(relevantDigits, suffixIndex, digitsBeforeDecimal);

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Gets the relevant amount of digits from the beginning of the number.
        /// </summary>
        /// <param name="suffixIndex">
        /// How many times the returned value would have to be multiplied by 1000 to get close to the input <paramref name="number"/> again.
        /// This equals the index in the suffix array for the result number.
        /// </param>
        /// <param name="digitsBeforeDecimal">
        /// How many times the returned value would have to be divided by 10 to get close to the input <paramref name="number"/> again.
        /// This equals the insertion index for the decimal separator in the returned number.
        /// </param>
        /// <param name="ensureLargerThanInputNumber">if <c>false</c>, the result will be less than or equal to the first digits of the <paramref name="number"/>. If <c>true</c>, it will be greater or equal.</param>
        private uint GetRelevantPositiveDigits(long number, out ushort suffixIndex, out byte digitsBeforeDecimal, bool ensureLargerThanInputNumber = false)
        {
            var digits = number;

            suffixIndex = 0;
            digitsBeforeDecimal = 0;
            var removedFactor = 1;
            do
            {
                digits /= 10;
                removedFactor *= 10;
                digitsBeforeDecimal++;
                if (digitsBeforeDecimal > 3)
                {
                    suffixIndex++;
                    digitsBeforeDecimal = 1;
                }
            }
            while (digits >= 1000);

            if (ensureLargerThanInputNumber && digits * removedFactor < number)
            {
                digits++;
                if (digits >= 1000 && digits.GetDigitCount() <= visibleDigitCount)
                {
                    digits /= 10;
                    digitsBeforeDecimal++;
                    if (digitsBeforeDecimal > visibleDigitCount)
                    {
                        suffixIndex++;
                        digitsBeforeDecimal = 1;
                    }
                }
            }

            return (uint)digits;
        }

        private void ComposeString(uint relevantDigits, ushort suffixIndex, byte digitsBeforeDecimal)
        {
            var digitsString = relevantDigits.ToString();
            if (digitsString.Length <= visibleDigitCount)
            {
                stringBuilder.Append(digitsString, 0, digitsBeforeDecimal);
                var fractionalDigitCount = digitsString.Length - digitsBeforeDecimal;
                if (fractionalDigitCount > 0)
                {
                    // Trim trailing zeros from the fractional part
                    var fractionalPart = digitsString.Substring(digitsBeforeDecimal);
                    fractionalPart = fractionalPart.TrimEnd('0');
                    
                    if (fractionalPart.Length > 0)
                    {
                        stringBuilder.Append(decimalSeparator);
                        stringBuilder.Append(fractionalPart);
                    }
                }
            }
            else
            {
                stringBuilder.Append(digitsString);
            }

            stringBuilder.Append(suffixProvider(suffixIndex));
        }
    }
}
