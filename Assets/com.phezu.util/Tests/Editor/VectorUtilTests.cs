using UnityEngine;
using NUnit.Framework;

namespace Phezu.Util.Editor.Tests {

    public class VectorUtilTests
    {
        private const float TOLERANCE = 0.01f;

        [TestCase(1f, 0f, 90f, VectorUtil.AngleType.Degrees, 0f, 1f)]
        [TestCase(1f, 1f, 90f, VectorUtil.AngleType.Degrees, -1f, 1f)]
        [TestCase(-1f, -1f, -90f, VectorUtil.AngleType.Degrees, -1f, 1f)]
        [TestCase(1f, 0f, Mathf.PI, VectorUtil.AngleType.Radians, -1f, 0f)]
        public void RotateVectorTest(float x, float y, float angle, VectorUtil.AngleType angleType, float outX, float outY) {
            Vector2 vector = new(x, y);
            Vector2 rotated = VectorUtil.RotateVector(vector, angle, angleType);

            Assert.AreEqual(outX, rotated.x, TOLERANCE);
            Assert.AreEqual(outY, rotated.y, TOLERANCE);
        }

    }
}