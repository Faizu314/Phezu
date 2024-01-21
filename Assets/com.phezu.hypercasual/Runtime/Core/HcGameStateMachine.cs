using System.Collections.Generic;

namespace Phezu.HyperCasualTemplate {
    public class HcGameStateMachine {

        private List<HcGameStateListener> m_Listeners;
        private HcGameState m_CurrState;
        private HcGameState m_PrevState;

        public HcGameState CurrState => m_CurrState;
        public HcGameState PrevState => m_PrevState;

        public HcGameStateMachine() {
            m_Listeners = new();
            m_CurrState = m_PrevState = HcGameState.Initializing;
        }

        public void Subscribe(HcGameStateListener listener) {
            m_Listeners.Add(listener);

            //listener.OnGameStateChanged(m_CurrState, m_PrevState);
        }

        public void Unsubscribe(HcGameStateListener listener) {
            m_Listeners.Remove(listener);
        }

        public void SetState(HcGameState state) {
            m_PrevState = m_CurrState;
            m_CurrState = state;

            OnStateChanged();
        }

        private void OnStateChanged() {
            foreach (var listener in m_Listeners)
                listener.OnGameStateChanged(m_CurrState, m_PrevState);
        }
    }
}