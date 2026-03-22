




using GdUnit4;
using static GdUnit4.Assertions;

namespace TestNS
{
    [TestSuite]
    public class Temp
    {

        [TestCase]
        public void success()
        {
            AssertBool(true).IsTrue();
        }

        [TestCase]
        public void failed()
        {
            AssertBool(false).IsTrue();
        }
    }
}