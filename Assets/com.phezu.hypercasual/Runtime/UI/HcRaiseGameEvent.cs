using UnityEngine;

namespace Phezu.HyperCasualTemplate {

    [AddComponentMenu("Phezu/Hyper Casual Template/UI/Hc Raise Game Event")]
    public class HcRaiseGameEvent : MonoBehaviour {

        private enum Event {
            OnLose,
            OnWin,
            RestartButtonPress ,
            NextLevelButtonPress,
            LevelSelectMenuPress,
            PauseButtonPress,
            ResumeButtonPress
        }

        [SerializeField] private Event m_Event;

        public void _RaiseEvent() {
            switch (m_Event) {
                case Event.OnLose:
                    HcGameManager.Instance.OnPlayerDead();
                    break;
                case Event.OnWin:
                    HcGameManager.Instance.OnPlayerWon();
                    break;
                case Event.RestartButtonPress:
                    HcGameManager.Instance.OnRestartButtonPressed();
                    break;
                case Event.NextLevelButtonPress:
                    HcGameManager.Instance.OnNextLevelButtonPressed();
                    break;
                case Event.LevelSelectMenuPress:
                    HcGameManager.Instance.OnLevelSelectMenuButtonPressed();
                    break;
                case Event.PauseButtonPress:
                    HcGameManager.Instance.OnPauseButtonPressed();
                    break;
                case Event.ResumeButtonPress:
                    HcGameManager.Instance.OnResumeButtonPressed();
                    break;
            }
        }
    }
}
