using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1;
using System.IO;

namespace NUnit.TestsPen
{
    [TestFixture]
    public class TestClass
    {
        private string result;
        private Pen pen;

        [SetUp]
        public void Init()
        {
            result = "";
        }

        [TearDown]
        public void Dispose()
        {
            pen = null;
        }

        static readonly object[] DataForTestPenConstructors =
        {
            new object[] {  8, null, null, "123456789" },
            new object[] {  8, 1.0, null, "123456789"},
            new object[] {  8, 2.0, "RED", "123456789"}
        };

        [Test]
        [TestCaseSource("DataForTestPenConstructors")]
        public void TestPenConstructors(int sizeInk, object sizeLetter, string color, string word)
        {
            if (sizeLetter == null)
            {
                pen = new Pen(sizeInk);
            }
            else if (color == null)
            {
                pen = new Pen(sizeInk, Convert.ToDouble(sizeLetter));
            }
            else
            {
                pen = new Pen(sizeInk, Convert.ToDouble(sizeLetter), color);
            }
            Assert.NotNull(pen);
        }

        static object[] DataForTestPenGetColor =
        {
            new object[] {  1000, 1.0, "RED"},
            new object[] {  1000, 1.0, "BLUE"}
        };

        [Test]
        [TestCaseSource("DataForTestPenGetColor")]
        public void TestPenGetColor(int sizeInk, double sizeLetter, string color)
        {
            pen = new Pen(sizeInk, sizeLetter, color);
            result = pen.getColor();
            Assert.AreEqual(color, result);
        }

        static object[] DataForTestPenIsWorkTrue =
        {
            new object[] {  100, true },
            new object[] { -100, false },
            new object[] {    0, false }
        };

        [Test]
        [TestCaseSource("DataForTestPenIsWorkTrue")]
        public void TestPenIsWorkTrue(int sizeInk, bool expectedResult)
        {
            pen = new Pen(sizeInk);
            bool actualResult = pen.isWork();
            Assert.AreEqual(expectedResult, actualResult);
        }

        static object[] DataForTestPenWrite =
        {
            new object[] {  100, 1.0, "123456789", 9 },
            new object[] {  6, 1.0, "123456789", 6 },
            new object[] {  6, 2.0, "123456789", 3 },
            new object[] {  100, 2.0, "123456789", 9 },
            new object[] {  10, 0.0, "123456789", 0 },
            new object[] {  10, -1.0, "123456789", 0 },
            new object[] {  100, 20.0, "123456789", 5 }
        };

        [Test]
        [TestCaseSource("DataForTestPenWrite")]
        public void TestPenWrite(int sizeInk, double sizeLetter, string word, int expectedResult)
        {
            pen = new Pen(sizeInk, sizeLetter);
            try
            {
                result = pen.write(word);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                Assert.Fail(ex.Message);
            }
            Assert.AreEqual(expectedResult, result.Length);
        }

        [TestCase(100, 2.0, "RED", "log.txt")]
        public void TestPenDoSomethingElse(int sizeInk, double sizeLetter, string color, string nameTxtFile)
        {
            result = "";
            pen = new Pen(sizeInk, sizeLetter, color);
            using (TextWriter writer = File.CreateText(nameTxtFile))
            {
                TextWriterTraceListener trcListener = new TextWriterTraceListener(writer);
                Debug.Listeners.Add(trcListener);
                pen.doSomethingElse();
                Debug.Flush();
                trcListener.Close();
            }
            FileStream file = new FileStream(nameTxtFile, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            using (StreamReader sr = new StreamReader(file, System.Text.Encoding.Default))
            {
                while (!sr.EndOfStream)
                {
                    result = sr.ReadLine();
                }
            }
            file.Close();
            File.Delete(nameTxtFile);
            Assert.AreEqual(color, result);
        }
    }
}
