using UnityEngine;
using UnityEngine.UI;
using Phezu.Audio;

namespace Phezu.HyperCasualTemplate {

    [RequireComponent(typeof(Button))]
    [AddComponentMenu("Phezu/Hyper Casual Template/Button Press Sound")]
    public class HcButtonPressSound : MonoBehaviour
    {
        [SerializeField] private Sound m_OnPressSound;

        private void Awake() {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick() {
            AudioPlayer.Instance.PlaySound(m_OnPressSound);
        }
    }
}