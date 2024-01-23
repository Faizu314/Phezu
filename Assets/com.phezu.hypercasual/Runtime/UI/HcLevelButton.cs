using UnityEngine;
using UnityEngine.UI;

namespace Phezu.HyperCasualTemplate {

    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Phezu/Hyper Casual Template/UI/Hc Level Button")]
    public class HcLevelButton : MonoBehaviour {
        [SerializeField] private int m_LevelIndex;

        private void Awake() {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick() {
            HcGameManager.Instance.OnLevelSelected(m_LevelIndex);
        }
    }
}