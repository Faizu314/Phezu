using UnityEngine;
using Phezu.Audio;

namespace Phezu.HyperCasualTemplate {

    [AddComponentMenu("Phezu/Hyper Casual Template/Sound/Hc Music Manager")]
    public class HcMusicManager : HcGameStateListener {

        [SerializeField][Range(0f, 1f)] private float m_VolumeMultiplierInLevels;

        public override void OnGameStateChanged(HcGameState curr, HcGameState prev) {
            base.OnGameStateChanged(curr, prev);

            float volume;

            if (curr == HcGameState.Playing)
                volume = m_VolumeMultiplierInLevels;
            else
                volume = 1f;

            MusicPlayer.Instance.SetVolumeLevel(volume);
        }
    }
}
