using UnityEngine;

namespace Phezu.HyperCasualTemplate {

    public class HcGameStateListener : MonoBehaviour {

        private bool m_IsSubbed = false;
        protected HcGameState m_CurrentState;
        protected HcGameState m_PrevState;

        protected virtual void Awake() {
            if (HcGameManager.Instance == null)
                return;
            HcGameManager.Instance.SubscribeToGameStateMachine(this);
            m_IsSubbed = true;
            m_CurrentState = HcGameState.Initializing;
            m_PrevState = HcGameState.Initializing;
        }

        protected virtual void OnDestroy() {
            if (m_IsSubbed && HcGameManager.Instance != null)
                HcGameManager.Instance.UnsubscribeToGameStateMachine(this);
        }

        public virtual void OnGameStateChanged(HcGameState curr, HcGameState prev) {
            m_CurrentState = curr;
            m_PrevState = prev;

            switch (curr) {
                case HcGameState.Initializing:
                    OnInitialization();
                    break;
                case HcGameState.MainMenu:
                    OnMainMenuLoaded();
                    break;
                case HcGameState.Paused:
                    OnPause();
                    break;
                case HcGameState.Win:
                    OnWin();
                    break;
                case HcGameState.Lose:
                    OnLose();
                    break;
                case HcGameState.Unloaded:
                    if (prev != HcGameState.MainMenu && prev != HcGameState.Initializing)
                        OnLevelEnd();
                    break;
                case HcGameState.Playing:
                    if (prev == HcGameState.Unloaded)
                        OnLevelStart();
                    else if (prev == HcGameState.Paused)
                        OnResume();
                    else if (prev == HcGameState.Lose)
                        OnLevelRestart();
                    break;
            }
        }

        protected virtual void OnInitialization() { }
        protected virtual void OnMainMenuLoaded() { }
        protected virtual void OnLevelStart() { }
        protected virtual void OnLevelRestart() { }
        protected virtual void OnLevelEnd() { }
        protected virtual void OnPause() { }
        protected virtual void OnResume() { }
        protected virtual void OnWin() { }
        protected virtual void OnLose() { }
    }
}