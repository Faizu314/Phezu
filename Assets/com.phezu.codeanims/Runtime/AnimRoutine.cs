using System;
using System.Collections;
using UnityEngine;

namespace Phezu.CodeAnims {

    [Serializable]
    public struct AnimInput {
        public Vector3 BasePos;
        public float BaseZRot;
        public Vector3 BaseScale;
        public Transform Output;
    }

    [Serializable]
    public struct AnimParams {
        public BobData XPosAnim;
        public BobData YPosAnim;
        public BobData ZPosAnim;

        public BobData ZRotAnim;

        public BobData XScaleAnim;
        public BobData YScaleAnim;
        public BobData ZScaleAnim;

        public float MasterAmplitude;
        public float MasterFrequency;
    }

    public static class AnimRoutine {

        public static IEnumerator BeginAnim(this MonoBehaviour mono, AnimInput input, AnimParams parameters, bool useUnscaledTime = false) {
            float t;
            if (useUnscaledTime)
                t = Time.unscaledDeltaTime * parameters.MasterFrequency;
            else
                t = Time.deltaTime * parameters.MasterFrequency;

            while (t < 1f) {
                input.Output.position = input.BasePos + (new Vector3(parameters.XPosAnim.Evaluate(t), parameters.YPosAnim.Evaluate(t), parameters.ZPosAnim.Evaluate(t)) * parameters.MasterAmplitude);
                var z = input.BaseZRot + (parameters.ZRotAnim.Evaluate(t) * parameters.MasterAmplitude);
                input.Output.localScale = input.BaseScale + (new Vector3(parameters.XScaleAnim.Evaluate(t), parameters.YScaleAnim.Evaluate(t), parameters.ZScaleAnim.Evaluate(t)) * parameters.MasterAmplitude);

                input.Output.rotation = new(0f, 0f, z, 1f);

                if (useUnscaledTime)
                    t += Time.unscaledDeltaTime * parameters.MasterFrequency;
                else
                    t += Time.deltaTime * parameters.MasterFrequency;

                yield return null;
            }
        }
    }
}