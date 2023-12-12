using UnityEngine;
using UnityEngine.InputSystem;
using Phezu.Util;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Phezu.HyperCasualTemplate {

    [DefaultExecutionOrder(-1)]
    public class HcInputManager : Singleton<HcInputManager> {

        [SerializeField] private GraphicRaycaster m_UIRaycaster;

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
        }

        private void OnEnable() {
            m_GameInput.PlayerControls.Enable();
        }

        private void OnDisable() {
            m_GameInput.PlayerControls.Disable();
        }

        private void Start() {
            m_GameInput.PlayerControls.PrimaryDown.started += ctx => OnPrimaryDownRaw(ctx);
            m_GameInput.PlayerControls.PrimaryDown.canceled += ctx => OnPrimaryUpRaw(ctx);

            m_Camera = Camera.main;
        }

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

        public Vector2 GetPrimaryPosition() {
            Vector3 worldPos = m_GameInput.PlayerControls.PrimaryPosition.ReadValue<Vector2>();
            worldPos.z = m_Camera.nearClipPlane;
            worldPos = m_Camera.ScreenToWorldPoint(worldPos);

            return worldPos;
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
            if (m_UIRaycaster != null) {
                m_Results.Clear();

                m_UIRaycaster.Raycast(m_PointerData, m_Results);

                overPauseMenu = m_Results.Count > 0;
            }

            return overHUD || overPauseMenu;
        }
    }
}