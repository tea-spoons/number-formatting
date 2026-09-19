
namespace TeaSpoons.NumberFormatting
{
    using System;
    using System.Globalization;

    /// <summary>
    /// An <see cref="ILongFormatter"/> that just adds thousands seperators.
    /// </summary>
    public class ExactLongFormatter : INumberFormatter<long>
    {
        private const char defaultThousandSeperator = ',';

        private readonly Func<long, string> format;

        public ExactLongFormatter(char thousandsSeperator = defaultThousandSeperator)
        {
            if (thousandsSeperator == defaultThousandSeperator)
            {
                format = FormatWithDots;
            }
            else
            {
                format = amount => FormatWithDots(amount).Replace(defaultThousandSeperator, thousandsSeperator);
            }
        }

        private string FormatWithDots(long number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        public string FormatAmount(long amount)
        {
            return format(amount);
        }

        public string FormatCost(long cost)
        {
            return format(cost);
        }
    }
}
