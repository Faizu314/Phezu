using UnityEngine;

namespace Phezu.HyperCasualTemplate {

    public class HcSwipeDetection : MonoBehaviour {
        [SerializeField] private float m_MinDistance;
        [SerializeField] private float m_MinDuration;

        private HcInputManager m_InputManager;

        private Vector2 m_StartPos;
        private Vector2 m_EndPos;
        private float m_StartTime;
        private float m_EndTime;

        private void Awake() {
            m_InputManager = HcInputManager.Instance;
        }

        private void OnEnable() {
            m_InputManager.OnPrimaryDown += OnPrimaryDown;
            m_InputManager.OnPrimaryUp += OnPrimaryUp;
        }

        private void OnDisable() {
            m_InputManager.OnPrimaryDown -= OnPrimaryDown;
            m_InputManager.OnPrimaryUp -= OnPrimaryUp;
        }

        private void OnPrimaryDown(Vector2 position, float time) {
            m_StartPos = position;
            m_StartTime = time;
        }

        private void OnPrimaryUp(Vector2 position, float time) {
            m_EndPos = position;
            m_EndTime = time;

            DetectSwipe();
        }

        private void DetectSwipe() {
            if ((m_EndTime - m_StartTime) > m_MinDuration ||
                (Vector2.Distance(m_StartPos, m_EndPos) < m_MinDistance))
                return;

            Debug.DrawLine(m_StartPos, m_EndPos, Color.red, 4f);

            if (m_EndPos.x > m_StartPos.x)
                HcInputEvents.InvokeSwipeRight();
            else if (m_EndPos.x < m_StartPos.x)
                HcInputEvents.InvokeSwipeLeft();
        }
    }
}