using UnityEngine;
using UnityEngine.EventSystems;

namespace Phezu.HyperCasualTemplate {

    [AddComponentMenu("Phezu/Hyper Casual Template/Hud Button")]
    public class HudButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

        [HudButton] public int m_ButtonType;

        private HcInputManager m_InputManager;

        private void Start() {
            m_InputManager = HcInputManager.Instance;
        }

        public void OnPointerDown(PointerEventData eventData) {
            m_InputManager.SetHudButton(m_ButtonType, true);
        }

        public void OnPointerUp(PointerEventData eventData) {
            m_InputManager.SetHudButton(m_ButtonType, false);
        }
    }
}
