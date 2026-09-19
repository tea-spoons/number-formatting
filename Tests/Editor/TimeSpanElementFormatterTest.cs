
namespace TeaSpoons.NumberFormatting.Editor.Tests
{
    using NUnit.Framework;
    using System;

    public class TimeSpanElementFormatterTest
    {
        private TimeSpanElementsFormatter formatter;
        private TimeSpanElementsFormatter precisionFormatter;

        [SetUp]
        public void SetUp()
        {
            formatter = new TimeSpanElementsFormatter("s", "m", "h", "d", "0", true, false);
            precisionFormatter = new TimeSpanElementsFormatter("s", "m", "h", "d", "0", true, true);
        }

        [TestCase(0, true, "0")]
        [TestCase(10, true, "10s")]
        [TestCase(100, true, "1m 40s")]
        [TestCase(100, false, "1m 40s")]
        [TestCase(120, true, "2m 00s")]
        [TestCase(120, false, "2m")]
        [TestCase(60 * 60, true, "1h 00m 00s")]
        [TestCase(60 * 60, false, "1h")]
        [TestCase(60 * 60 + 2, true, "1h 00m 02s")]
        [TestCase(60 * 60 + 2, false, "1h 02s")]
        [TestCase(60 * 60 * 24 + 2, true, "1d 00h 00m 02s")]
        [TestCase(60 * 60 * 24 + 2, false, "1d 02s")]
        public void TimeSpanElementsFormatter(long input, bool includeSmallerZeroes, string output)
        {
            Assert.AreEqual(output, formatter.Format(TimeSpan.FromSeconds(input), includeSmallerZeroes));
        }
        
        [TestCase(1000, "1s")]
        [TestCase(100, "0.1s")]
        [TestCase(1500, "1.5s")]
        [TestCase(123, "0.123s")]
        [TestCase(500, "0.5s")]
        [TestCase(50, "0.05s")]
        public void FormatSubSecondPrecision(int milliseconds, string expected)
        {
            var result = precisionFormatter.FormatAmount(TimeSpan.FromMilliseconds(milliseconds));
            Assert.AreEqual(expected, result);
        }
        
        [TestCase(1000, "1s")]
        [TestCase(1500, "1s")]
        [TestCase(100, "0")]
        [TestCase(50, "0")]
        public void FormatSubSecondNoPrecision(int milliseconds, string expected)
        {
            var result = formatter.FormatAmount(TimeSpan.FromMilliseconds(milliseconds));
            Assert.AreEqual(expected, result);
        }
        
        [Test]
        public void FormatSubSecondPrecisionWithLargerUnits()
        {
            var ts = new TimeSpan(3, 2, 0, 7, 100);
            Assert.AreEqual("3d 02h 07.1s", precisionFormatter.FormatAmount(ts));
            Assert.AreEqual("3d 02h 00m 07.1s", precisionFormatter.Format(ts, includeSmallerZeroes: true));
        }

        [Test]
        public void FormatSubSecondNoPrecisionWithLargerUnits()
        {
            var ts = new TimeSpan(3, 2, 0, 7, 100);
            Assert.AreEqual("3d 02h 07s", formatter.FormatAmount(ts));
        }
    }
}
