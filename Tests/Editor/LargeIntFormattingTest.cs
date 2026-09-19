
#if LARGE_NUMBERS
namespace TeaSpoons.NumberFormatting.Editor.Tests
{
    using NUnit.Framework;
    using System;

    public class LargeIntFormattingTest
    {
        [TestCase(0, "0")]
        [TestCase(10, "10")]
        [TestCase(100, "100")]
        [TestCase(1000, "1K")]
        [TestCase(1001, "1K")]
        [TestCase(1010, "1.01K")]
        [TestCase(1011, "1.01K")]
        [TestCase(1100, "1.1K")]
        [TestCase(1101, "1.1K")]
        [TestCase(1110, "1.11K")]
        [TestCase(1111, "1.11K")]
        [TestCase(10000, "10K")]
        [TestCase(10100, "10.1K")]
        [TestCase(100000, "100K")]
        [TestCase(999000, "999K")]
        [TestCase(999001, "999K")]
        [TestCase(999999, "999K")]
        [TestCase(1000000, "1M")]
        [TestCase(-10, "-10")]
        [TestCase(-999000, "-999K")]
        [TestCase(-999001, "-1M")]
        [TestCase(-999999, "-1M")]
        [TestCase(-1000000, "-1M")]
        public void SuffixLargeIntFormatterForAmount(long input, string output)
        {
            var formatter = new SuffixLargeIntFormatter(NumberSuffixProviders.TrillionThenDoubleLetter);

            Assert.AreEqual(output, formatter.FormatAmount(input));
        }

        [TestCase(0, "0")]
        [TestCase(10, "10")]
        [TestCase(100, "100")]
        [TestCase(1000, "1K")]
        [TestCase(1001, "1.01K")]
        [TestCase(1010, "1.01K")]
        [TestCase(1011, "1.02K")]
        [TestCase(1100, "1.1K")]
        [TestCase(1101, "1.11K")]
        [TestCase(1110, "1.11K")]
        [TestCase(1111, "1.12K")]
        [TestCase(10000, "10K")]
        [TestCase(10100, "10.1K")]
        [TestCase(100000, "100K")]
        [TestCase(999000, "999K")]
        [TestCase(999001, "1M")]
        [TestCase(999999, "1M")]
        [TestCase(1000000, "1M")]
        [TestCase(-10, "-10")]
        [TestCase(-999000, "-999K")]
        [TestCase(-999001, "-999K")]
        [TestCase(-999999, "-999K")]
        [TestCase(-1000000, "-1M")]
        public void SuffixLongFormatterForCost(long input, string output)
        {
            var formatter = new SuffixLongFormatter(NumberSuffixProviders.TrillionThenDoubleLetter);

            Assert.AreEqual(output, formatter.FormatCost(input));
        }

        [Test]
        public void LargeIntMissingSuffix()
        {
            var formatter = new SuffixLargeIntFormatter(NoSuffixes);

            Assert.AreEqual('\u221e'.ToString(), formatter.FormatCost(new LargeNumbers.LargeInt(1, 5000)));
        }

        private string NoSuffixes(ushort index)
        {
            throw new ArgumentOutOfRangeException();
        }
    }
}
#endif
