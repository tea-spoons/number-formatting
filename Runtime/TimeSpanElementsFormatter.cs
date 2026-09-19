namespace TeaSpoons.NumberFormatting
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// Formats a <see cref="TimeSpan"/> like this: "1h 22m"
    /// </summary>
    public class TimeSpanElementsFormatter : INumberFormatter<TimeSpan>
    {
        private static readonly StringBuilder stringBuilder = new();

        private readonly string secondsSuffix;
        private readonly string minutesSuffix;
        private readonly string hoursSuffix;
        private readonly string daysSuffix;
        private readonly string zero;
        private readonly bool leadingZero;
        private readonly bool subSecondPrecision;

        public TimeSpanElementsFormatter() : this("s", "m", "h", "d", "0", true, false)
        {

        }

        /// <param name="zero">The string to return for a <see cref="TimeSpan.Zero"/>.</param>
        /// <param name="leadingZero">If <c>true</c>, all but the first number will have a leading zero for single-digit values.</param>
        /// <param name="subSecondPrecision">
        /// If <c>true</c>, the seconds element is rendered with up to millisecond precision
        /// (e.g. "0.1s", "1.5s") whenever the <see cref="TimeSpan"/> has a sub-second component.
        /// When <c>false</c> (the default), only whole seconds are shown and any sub-second
        /// remainder is dropped, so a span below one second formats as the <paramref name="zero"/> string.
        /// </param>
        public TimeSpanElementsFormatter(string secondsSuffix,
            string minutesSuffix,
            string hoursSuffix,
            string daysSuffix,
            string zero,
            bool leadingZero,
            bool subSecondPrecision = false)
        {
            this.secondsSuffix = secondsSuffix;
            this.minutesSuffix = minutesSuffix;
            this.hoursSuffix = hoursSuffix;
            this.daysSuffix = daysSuffix;
            this.zero = zero;
            this.leadingZero = leadingZero;
            this.subSecondPrecision = subSecondPrecision;
        }

        public string Format(TimeSpan value, bool includeSmallerZeroes)
        {
            var includeZeroes = false;
            var first = true;
            var totalDays = (int)value.TotalDays;
            if (totalDays > 0)
            {
                stringBuilder.Append(totalDays).Append(daysSuffix);
                includeZeroes = includeSmallerZeroes;
                first = false;
            }
            if (includeZeroes || value.Hours > 0)
            {
                if (!first)
                {
                    stringBuilder.Append(" ");
                    if (leadingZero && value.Hours < 10)
                    {
                        stringBuilder.Append('0');
                    }
                }
                stringBuilder.Append(value.Hours).Append(hoursSuffix);
                includeZeroes = includeSmallerZeroes;
                first = false;
            }
            if (includeZeroes || value.Minutes > 0)
            {
                if (!first)
                {
                    stringBuilder.Append(" ");
                    if (leadingZero && value.Minutes < 10)
                    {
                        stringBuilder.Append('0');
                    }
                }
                stringBuilder.Append(value.Minutes).Append(minutesSuffix);
                includeZeroes = includeSmallerZeroes;
                first = false;
            }

            var hasSubSecond = subSecondPrecision && value.Milliseconds > 0;
            if (includeZeroes || value.Seconds > 0 || hasSubSecond)
            {
                if (!first)
                {
                    stringBuilder.Append(" ");
                    if (leadingZero && value.Seconds < 10)
                    {
                        stringBuilder.Append('0');
                    }
                }

                if (hasSubSecond)
                {
                    var seconds = value.Seconds + value.Milliseconds / 1000.0;
                    stringBuilder.Append(seconds.ToString("0.###", CultureInfo.InvariantCulture));
                }
                else
                {
                    stringBuilder.Append(value.Seconds);
                }

                stringBuilder.Append(secondsSuffix);
                first = false;
            }

            if (first)
            {
                stringBuilder.Append(zero);
            }

            var result = stringBuilder.ToString();
            stringBuilder.Clear();
            return result;
        }

        public string FormatAmount(TimeSpan amount)
        {
            return Format(amount, false);
        }

        public string FormatCost(TimeSpan cost)
        {
            return FormatAmount(cost);
        }
    }
}
