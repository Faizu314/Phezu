using System;

namespace Phezu.HyperCasualTemplate {

    public static class HcInputEvents {

        public static event Action OnSwipeRight;
        public static event Action OnSwipeLeft;
        public static event Action OnTap;

        public static void InvokeSwipeRight() {
            OnSwipeRight?.Invoke();
        }
        public static void InvokeSwipeLeft() {
            OnSwipeLeft?.Invoke();
        }

        public static void InvokeTap() {
            OnTap?.Invoke();
        }
    }
}