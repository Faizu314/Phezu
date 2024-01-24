using UnityEngine;

namespace Phezu.Util {

    public static class VectorUtil {

        public enum AngleType { Degrees, Radians }

        public static Vector2 VectorFromAngle(float angle, AngleType angleType = AngleType.Degrees) {
            float angleInRadians = angleType == AngleType.Radians ? angle : angle * Mathf.Deg2Rad;

            return new(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
        }

        public static Vector2 RotateVector(Vector2 vector, float rotation, AngleType rotationType = AngleType.Degrees) {
            float rotInRadians = rotationType == AngleType.Radians ? rotation : rotation * Mathf.Deg2Rad;

            float angle = Vector2.SignedAngle(Vector2.right, vector) * Mathf.Deg2Rad;
            angle += rotInRadians;

            return VectorFromAngle(angle, AngleType.Radians) * vector.magnitude;
        }
    }
}