using System.Collections;
using UnityEngine;
using Phezu.Util;

namespace Phezu.HyperCasualTemplate {

    public class HcTimeManager : Singleton<HcTimeManager> {

        [SerializeField] private float m_LerpTime;

        private float m_CurrentTimeScale = 1f;
        private float m_CurrentFixedTimeStep;

        [SerializeField]
        [Range(0f, 1f)] private float debug;

        private float m_TimeScaleTarget = 1f;
        private float m_OriginalFixedTimeStep;

        protected virtual void Awake() {
            m_OriginalFixedTimeStep = Time.fixedDeltaTime;
            m_CurrentFixedTimeStep = m_OriginalFixedTimeStep;

            HcGameManager.OnLose += SlowTime;
            HcGameManager.OnWin += SlowTime;
            HcGameManager.OnPause += PauseTime;
            HcGameManager.OnResume += ResetTime;
            HcGameManager.OnLevelRestart += ResetTime;
            HcGameManager.OnLevelEnd += ResetTime;
        }

        protected override void OnDestroy() {
            HcGameManager.OnLose -= SlowTime;
            HcGameManager.OnWin -= SlowTime;
            HcGameManager.OnPause -= PauseTime;
            HcGameManager.OnResume -= ResetTime;
            HcGameManager.OnLevelRestart -= ResetTime;
            HcGameManager.OnLevelEnd -= ResetTime;
        }

        protected virtual void Update() {
            Time.timeScale = m_CurrentTimeScale;
            Time.fixedDeltaTime = m_CurrentFixedTimeStep;
            debug = m_CurrentTimeScale;
        }

        private void SlowTime() {
            m_TimeScaleTarget = 0.1f;
            StartCoroutine(nameof(SlowMo_Co));
        }

        private void PauseTime() {
            StopAllCoroutines();
            m_CurrentTimeScale = 0f;
            m_CurrentFixedTimeStep = m_OriginalFixedTimeStep * m_CurrentTimeScale;
        }

        private void ResetTime() {
            StopAllCoroutines();
            m_CurrentTimeScale = 1f;
            m_CurrentFixedTimeStep = m_OriginalFixedTimeStep * m_CurrentTimeScale;
        }

        public void SetTimeScaleTarget(float timeScaleTarget, float lerpK) {
            m_TimeScaleTarget = timeScaleTarget;
            StartCoroutine(nameof(SlowMo_Co));
        }

        public void SetTimeScaleTarget(float timeScale) {
            m_TimeScaleTarget = timeScale;
        }

        public void SetTimeScale(float timeScale) {
            m_TimeScaleTarget = timeScale;
            m_CurrentTimeScale = timeScale;
            m_CurrentFixedTimeStep = timeScale * m_OriginalFixedTimeStep;
        }

        protected virtual IEnumerator SlowMo_Co() {
            float initialScale = m_CurrentTimeScale;
            float initialTime = Time.time;

            while (Mathf.Abs(m_CurrentTimeScale - m_TimeScaleTarget) > 0.01f) {
                float t = Mathf.Clamp01((Time.time - initialTime) / m_LerpTime);
                m_CurrentTimeScale += Mathf.Lerp(initialScale, m_TimeScaleTarget, t);
                m_CurrentFixedTimeStep = m_OriginalFixedTimeStep * m_CurrentTimeScale;

                yield return null;
            }

            m_CurrentTimeScale = m_TimeScaleTarget;
            m_CurrentFixedTimeStep = m_OriginalFixedTimeStep * m_CurrentTimeScale;
        }
    }
}