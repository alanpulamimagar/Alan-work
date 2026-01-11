using Microsoft.VisualStudio.TestTools.UnitTesting;
using booseapp;

namespace booseapp.tests
{
    [TestClass]
    public class CommandCanvasTests
    {
        [TestMethod]
        public void MoveTo_Sets_Xpos_Ypos()
        {
            using var canvas = new CommandCanvas();

            canvas.MoveTo(100, 50);

            Assert.AreEqual(100, canvas.Xpos);
            Assert.AreEqual(50, canvas.Ypos);
        }

        [TestMethod]
        public void DrawTo_Updates_Position()
        {
            using var canvas = new CommandCanvas();

            canvas.MoveTo(10, 10);
            canvas.DrawTo(40, 60);

            Assert.AreEqual(40, canvas.Xpos);
            Assert.AreEqual(60, canvas.Ypos);
        }

        [TestMethod]
        public void Reset_Sets_Position_To_Zero()
        {
            using var canvas = new CommandCanvas();

            canvas.MoveTo(200, 200);
            canvas.Reset();

            Assert.AreEqual(0, canvas.Xpos);
            Assert.AreEqual(0, canvas.Ypos);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Circle_Negative_Radius_Throws()
        {
            using var canvas = new CommandCanvas();

            canvas.Circle(-1, false);
        }

        [TestMethod]
        public void Set_Valid_Size_Does_Not_Throw()
        {
            using var canvas = new CommandCanvas();

            canvas.Set(800, 600);

            Assert.IsTrue(true);
        }
    }
}
