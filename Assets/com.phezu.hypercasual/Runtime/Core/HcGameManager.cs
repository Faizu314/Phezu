using System;
using System.Collections;
using UnityEngine;
using Phezu.Util;
using Phezu.SceneManagement;

namespace Phezu.HyperCasualTemplate {

    [DefaultExecutionOrder(-1)]
    [AddComponentMenu("Phezu/Hyper Casual Template/Core/Hc Game Manager")]
    public class HcGameManager : Singleton<HcGameManager> {

        [SerializeField] protected GameScenes m_GameScenes;
        [SerializeField] private int m_TargetFrameRate = 30;

#if UNITY_EDITOR
        [SerializeField] protected bool Test;
#endif

        protected HcGameStateMachine m_StateMachine;
        protected AsyncSceneLoader m_SceneLoader;

        protected int m_CurrentLevel = -1;
        public int CurrentLevel => m_CurrentLevel;

        public bool IsUserInLevel => m_CurrentLevel != -1;

        [Header("Debug")]
        [SerializeField] private HcGameState CurrentState;

        protected virtual void Awake() {
            Application.targetFrameRate = m_TargetFrameRate;

            m_StateMachine = new();
            m_StateMachine.SetState(HcGameState.Initializing);
        }

        protected virtual void Start() {
            m_SceneLoader = AsyncSceneLoader.Instance;
        }

        public virtual bool CanProceedToNextLevel() {
            return true;
        }

        public void SubscribeToGameStateMachine(HcGameStateListener listener) {
            m_StateMachine.Subscribe(listener);
        }

        public void UnsubscribeToGameStateMachine(HcGameStateListener listener) {
            m_StateMachine.Unsubscribe(listener);
        }

        protected virtual IEnumerator Init_Co() {
            yield return m_SceneLoader.LoadSceneAdditive(m_GameScenes.MainMenuScene);

            m_StateMachine.SetState(HcGameState.MainMenu);
            CurrentState = HcGameState.MainMenu;
        }

        #region Private Methods

        private IEnumerator LoadScene(string sceneName) {
            m_CurrentLevel = -1;
            yield return m_SceneLoader.LoadScene(sceneName);
        }

        private IEnumerator LoadLevel(int levelIndex) {
            if (m_GameScenes.LevelScenes.Count <= levelIndex)
                yield return LoadScene(m_GameScenes.LevelSelectionScene);
            else
                yield return LoadScene(m_GameScenes.LevelScenes[levelIndex].SceneName);

            m_CurrentLevel = levelIndex;
        }

        private IEnumerator LoadNextLevel() {
            if (m_CurrentLevel == -1)
                yield return null;
            yield return LoadLevel(m_CurrentLevel + 1);
        }

        #endregion

        #region Game Manager Routines

        public void OnPauseButtonPressed() {
            m_StateMachine.SetState(HcGameState.Paused);
            CurrentState = HcGameState.Paused;
            OnPause?.Invoke();
        }

        public void OnResumeButtonPressed() {
            m_StateMachine.SetState(HcGameState.Playing);
            CurrentState = HcGameState.Playing;
            OnResume?.Invoke();
        }

        public void OnRestartButtonPressed() {
            StartCoroutine(nameof(RestartButtonPressed_Co));
        }

        public void OnNextLevelButtonPressed() {
            if (!CanProceedToNextLevel())
                return;
            StartCoroutine(nameof(NextLevelButtonPressed_Co));
        }

        public void OnLevelSelectMenuButtonPressed() {
            StartCoroutine(nameof(LevelSelectMenuButtonPressed_Co));
        }

        public void OnLevelSelected(int levelIndex) {
            StartCoroutine(nameof(LevelSelected_Co), levelIndex);
        }

        public void OnPlayerDead() {
            StartCoroutine(nameof(PlayerDead_Co));
        }

        public void OnPlayerWon() {
            StartCoroutine(nameof(PlayerWon_Co));
        }


        protected virtual IEnumerator RestartButtonPressed_Co() {
            OnLevelRestart?.Invoke();
            m_StateMachine.SetState(HcGameState.Playing);
            CurrentState = HcGameState.Playing;

            yield return null;
        }

        protected virtual IEnumerator NextLevelButtonPressed_Co() {
            OnLevelEnd?.Invoke();
            m_StateMachine.SetState(HcGameState.Unloaded);
            CurrentState = HcGameState.Unloaded;

            yield return LoadNextLevel();

            OnLevelStart?.Invoke();
            m_StateMachine.SetState(HcGameState.Playing);
            CurrentState = HcGameState.Playing;
        }

        protected virtual IEnumerator LevelSelectMenuButtonPressed_Co() {
            OnResume?.Invoke();
            m_StateMachine.SetState(HcGameState.Unloaded);
            CurrentState = HcGameState.Unloaded;

            yield return LoadScene(m_GameScenes.LevelSelectionScene);

            m_StateMachine.SetState(HcGameState.MainMenu);
            CurrentState = HcGameState.MainMenu;
        }

        protected virtual IEnumerator LevelSelected_Co(int levelIndex) {
            m_StateMachine.SetState(HcGameState.Unloaded);
            CurrentState = HcGameState.Unloaded;

            yield return LoadLevel(levelIndex);

            OnLevelStart?.Invoke();
            m_StateMachine.SetState(HcGameState.Playing);
            CurrentState = HcGameState.Playing;
        }

        protected virtual IEnumerator PlayerDead_Co() {
            OnLose?.Invoke();
            m_StateMachine.SetState(HcGameState.Lose);
            CurrentState = HcGameState.Lose;

            yield return AfterPlayerLose();
        }

        protected virtual IEnumerator PlayerWon_Co() {
            OnWin?.Invoke();
            m_StateMachine.SetState(HcGameState.Win);
            CurrentState = HcGameState.Win;

            yield return AfterPlayerWon();
        }

        protected virtual IEnumerator AfterPlayerLose() { yield return null; }
        protected virtual IEnumerator AfterPlayerWon() { yield return null; }

        #endregion

        #region Game Events

        public static event Action OnLevelStart;
        public static event Action OnLevelRestart;
        public static event Action OnLevelEnd;
        public static event Action OnLose;
        public static event Action OnWin;
        public static event Action OnPause;
        public static event Action OnResume;

        #endregion
    }
}