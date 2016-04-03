using System.Globalization;
using NUnit.Framework;

namespace LibroLib.Tests.CommonTests
{
    public class StringExTests
    {
        [Test]
        [TestCase("   test ", "test", 3, 1)]
        [TestCase("   test", "test", 3, 0)]
        [TestCase("   test\r", "test", 3, 1)]
        public void TrimExTests(string text, string trimmed, int start, int end)
        {
            int s, e;
            string actualTrimmed = text.TrimEx(out s, out e);
            Assert.AreEqual(trimmed, actualTrimmed);
            Assert.AreEqual(start, s);
            Assert.AreEqual(end, e);
        }

        [Test]
        public void ConcatTest()
        {
            int[] values = new[] { 10, 20, 30 };
            Assert.AreEqual("10, 20, 30", values.Concat(x => x.ToString(CultureInfo.InvariantCulture), ", "));
        }

        [Test]
        [TestCase("pascal_case", "pascalCase")]
        [TestCase("camel_case", "CamelCase")]
        [TestCase("dbname", "DBName")]
        [TestCase("new_dbname", "newDBName")]
        public void UnderscoreNames(string expected, string original)
        {
            Assert.AreEqual(expected, original.ToUnderscoreName(CultureInfo.InvariantCulture));
        }
    }
}