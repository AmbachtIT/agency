using System.Numerics;
using Agency.Mathmatics;
using NUnit.Framework;

namespace Agency.Test.Mathmatics
{
    
    [TestFixture()]
    public class Segments
    {

        [Test()]
        public void TestLine()
        {
            var start = Vector3.Zero;
            var end = Vector3.UnitX;
            var line = Line.Between(start, end);
            Assert.AreEqual(1f, line.Length);
            TestProjection(line, -1f, new Vector3(-1, 0, 0));
            TestProjection(line, 0, new Vector3(0, 0, 0));
            TestProjection(line, 0, new Vector3(0, 1, 1));
            TestProjection(line, 0, new Vector3(0, 0, 1));
            TestProjection(line, 0, new Vector3(0, 1, 1));
            TestProjection(line, 0.5f, new Vector3(0.5f, 1, 1));
            TestProjection(line, 1f, new Vector3(1, 1, 1));
            TestProjection(line, 2f, new Vector3(2, 1, 1));
        }

        private void TestProjection(ISegment segment, float expectedAlpha, Vector3 v)
        {
            var alpha = segment.Project(v);
            Assert.AreEqual(expectedAlpha, alpha);
            var projected = segment.F(alpha);
            var alpha2 = segment.Project(projected);
            Assert.AreEqual(alpha, alpha2);
        }
        
    }
}