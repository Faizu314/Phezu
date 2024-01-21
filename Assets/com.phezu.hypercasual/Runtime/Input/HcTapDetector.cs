using UnityEngine;

namespace Phezu.HyperCasualTemplate {

    [AddComponentMenu("Phezu/Hyper Casual Template/Tap Detector")]
    public class HcTapDetector : MonoBehaviour {

        private HcInputManager m_InputManager;

        private void Awake() {
            m_InputManager = HcInputManager.Instance;
        }

        private void OnEnable() {
            m_InputManager.OnPrimaryDown += OnPrimaryDown;
        }

        private void OnDisable() {
            m_InputManager.OnPrimaryDown -= OnPrimaryDown;
        }

        private void OnPrimaryDown(Vector2 position, float time) {
            HcInputEvents.InvokeTap();
        }
    }
}