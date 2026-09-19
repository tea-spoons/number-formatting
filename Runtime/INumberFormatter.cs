
namespace TeaSpoons.NumberFormatting
{
    /// <summary>
    /// Formats numbers into strings.
    /// </summary>
    /// <typeparam name="T">The type of number to format.</typeparam>
    public interface INumberFormatter<T>
    {
        /// <summary>
        /// Formats the given number as an amount, rounding it down if digits need to be cut off.
        /// </summary>
        string FormatAmount(T amount);

        /// <summary>
        /// Formats the given number as a cost, rounding it up if digits need to be cut off.
        /// </summary>
        string FormatCost(T cost);
    }
}
