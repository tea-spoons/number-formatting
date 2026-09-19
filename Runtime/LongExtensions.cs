
namespace TeaSpoons.NumberFormatting
{
    internal static class LongExtensions
    {
        /// <summary>
        /// Returns the amount of digits in the number.
        /// </summary>
        public static byte GetDigitCount(this long value)
        {
            value = value < 0 ? -value : value;

            byte result = 1;
            while (value > 10)
            {
                value /= 10;
                result++;
            }

            return result;
        }
    }
}
