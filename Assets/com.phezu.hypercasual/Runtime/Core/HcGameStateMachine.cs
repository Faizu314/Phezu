using Phezu.Util;
using System.Collections.Generic;

namespace Phezu.HyperCasualTemplate {

    public class HcGameStateMachine {

        private ListBuckets<HcGameStateListener> m_Listeners;
        private List<int> m_Orders;
        private HcGameState m_CurrState;
        private HcGameState m_PrevState;

        public HcGameState CurrState => m_CurrState;
        public HcGameState PrevState => m_PrevState;

        public HcGameStateMachine() {
            m_Listeners = new();
            m_Orders = new();
            m_CurrState = m_PrevState = HcGameState.Initializing;
        }

        public void Subscribe(HcGameStateListener listener) {
            int order = listener.Order;
            m_Listeners.Add(listener, order);

            if (m_Orders.Contains(order))
                return;

            for (int i = 0; i < m_Orders.Count; i++) {
                if (m_Orders[i] > order) {
                    m_Orders.Insert(i, order);
                    return;
                }
            }

            m_Orders.Add(order);
        }

        public void Unsubscribe(HcGameStateListener listener) {
            m_Listeners.Remove(listener, listener.Order);

            var bucket = m_Listeners.Get(listener.Order);

            if (bucket == null || bucket.Count < 1)
                m_Orders.Remove(listener.Order);
        }

        public void SetState(HcGameState state) {
            m_PrevState = m_CurrState;
            m_CurrState = state;

            OnStateChanged();
        }

        private void OnStateChanged() {
            for (int i = 0; i < m_Orders.Count; i++) {
                var listeners = m_Listeners.Get(m_Orders[i]);

                if (listeners == null)
                    continue;

                foreach (var listener in listeners) {
                    listener.OnGameStateChanged(m_CurrState, m_PrevState);
                }
            }
        }
    }
}