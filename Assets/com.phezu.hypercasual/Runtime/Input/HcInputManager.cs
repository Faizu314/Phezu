using UnityEngine;
using UnityEngine.InputSystem;
using Phezu.Util;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Phezu.HyperCasualTemplate {

    [DefaultExecutionOrder(-1)]
    [AddComponentMenu("Phezu/Hyper Casual Template/Input/Hc Input Manager")]
    public class HcInputManager : Singleton<HcInputManager> {

        [SerializeField] private GraphicRaycaster m_PauseMenuRaycaster;
        [SerializeField] private string[] m_HudButtons;

        public delegate void PrimaryInput(Vector2 position, float time);
        public event PrimaryInput OnPrimaryDown;
        public event PrimaryInput OnPrimaryUp;

        private GameInput m_GameInput;
        private Camera m_Camera;
        private bool m_WasDownOnUI = false;

        [HideInInspector] public GraphicRaycaster CurrentHudRaycaster;
        private PointerEventData m_PointerData;
        private List<RaycastResult> m_Results = new();

        private void Awake() {
            m_GameInput = new();
            m_PointerData = new(EventSystem.current);

            if (m_HudButtons == null)
                return;
            m_HudButtonsState = new bool[m_HudButtons.Length];
        }

        private void OnEnable() {
            m_GameInput.PlayerControls.Enable();
        }

        private void OnDisable() {
            m_GameInput.PlayerControls.Disable();
        }

        private void Start() {
#if UNITY_EDITOR
            for (int i = 0; i < m_HudButtons.Length; i++)
                m_DebugInputs.Add(false);
#endif

            m_GameInput.PlayerControls.PrimaryDown.started += ctx => OnPrimaryDownRaw(ctx);
            m_GameInput.PlayerControls.PrimaryDown.canceled += ctx => OnPrimaryUpRaw(ctx);

            m_Camera = Camera.main;
        }

        #region HUD Buttons

#if UNITY_EDITOR

        [SerializeField] private List<bool> m_DebugInputs;

        private void Update() {
            for (int i = 0; i < m_DebugInputs.Count; i++)
                m_DebugInputs[i] = m_HudButtonsState[i];
        }
#endif

        public string[] HudButtons {
            get {
                string[] result = new string[m_HudButtons.Length];
                for (int i = 0; i < result.Length; i++)
                    result[i] = m_HudButtons[i];
                return result;
            }
        }

        private bool[] m_HudButtonsState;

        public void SetHudButton(int hudButtonIndex, bool state) {
            m_HudButtonsState[hudButtonIndex] = state;
        }

        public bool GetHudButton(int hudButtonIndex) {
            return m_HudButtonsState[hudButtonIndex];
        }

        public bool[] GetHudButtons() {
            bool[] buttons = new bool[m_HudButtonsState.Length];

            for (int i = 0; i < buttons.Length; i++) {
                buttons[i] = m_HudButtonsState[i];
            }

            return buttons;
        }

        #endregion

        private void OnPrimaryDownRaw(InputAction.CallbackContext context) {
            if (IsPointerOverUI()) {
                m_WasDownOnUI = true;
                return;
            }

            Vector3 worldPos = m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            worldPos.z = m_Camera.nearClipPlane;
            worldPos = m_Camera.ScreenToWorldPoint(worldPos);

            OnPrimaryDown?.Invoke(worldPos, (float)context.startTime);
        }

        private void OnPrimaryUpRaw(InputAction.CallbackContext context) {
            if (m_WasDownOnUI) {
                m_WasDownOnUI = false;
                return;
            }

            Vector3 worldPos = m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            worldPos.z = m_Camera.nearClipPlane;
            worldPos = m_Camera.ScreenToWorldPoint(worldPos);

            OnPrimaryUp?.Invoke(worldPos, (float)context.time);
        }

        public Vector3 GetPrimaryWorldPosition() {
            Vector3 worldPos = m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            worldPos.z = m_Camera.nearClipPlane;
            worldPos = m_Camera.ScreenToWorldPoint(worldPos);

            return worldPos;
        }

        /// <summary>
        /// Does not take safe area into account. (0, 0) => bottom left, (Screen.width, Screen.height) => top right
        /// </summary>
        public Vector2 GetPrimaryScreenPosition() {
            return m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
        }

        /// <summary>
        /// Does not take safe area into account. (0, 0) => bottom left, (1, 1) => top right
        /// </summary>
        public Vector2 GetPrimaryNormalizedScreenPosition() {
            var screenPos = GetPrimaryScreenPosition();

            screenPos.x /= Screen.width;
            screenPos.y /= Screen.height;

            return screenPos;
        }

        private bool IsPointerOverUI() {
            bool overHUD = false;
            bool overPauseMenu = false;

            m_PointerData.position = m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();

            if (CurrentHudRaycaster != null) {
                m_Results.Clear();

                CurrentHudRaycaster.Raycast(m_PointerData, m_Results);

                overHUD = m_Results.Count > 0;
            }
            if (m_PauseMenuRaycaster != null) {
                m_Results.Clear();

                m_PauseMenuRaycaster.Raycast(m_PointerData, m_Results);

                overPauseMenu = m_Results.Count > 0;
            }

            return overHUD || overPauseMenu;
        }
    }
}