using System;
using UnityEngine;

namespace Phezu.CodeAnims {

    [Serializable]
    public struct BobData {
        [Tooltip("Domain (0f, 1f)")]
        public AnimationCurve BobCurve;
        public float Amplitude;
        public float Frequency;

        public float Evaluate(float t) {
            return BobCurve.Evaluate((t * Frequency) % 1f) * Amplitude;
        }
    }
}