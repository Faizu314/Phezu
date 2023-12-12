using UnityEngine;
using Phezu.Util;
using System.Collections;

namespace Phezu.Audio {

    public class MusicPlayer : Singleton<MusicPlayer> {

        [SerializeField] private Sound m_MusicLoop;
        [SerializeField] private AudioSource m_AudioSource;
        [SerializeField] private float m_LerpSpeed;

        private float m_GlobalMusicVolume = 1f;
        public float GlobalMusicVolume {
            get {
                return m_GlobalMusicVolume;
            }
            set {
                m_GlobalMusicVolume = value;

                m_AudioSource.volume = m_MusicLoop.Volume * m_GlobalMusicVolume * m_VolumeMultiplier;
            }
        }

        private float m_VolumeMultiplier = 0f;
        private float m_MusicTime = 0f;

        private void Start() {
            m_VolumeMultiplier = 0f;
            SetVolumeLevel(1f);
            PlayMusic();
        }

        public void SetVolumeLevel(float volume) {
            StartCoroutine(nameof(SetVoumeLevel_Co), volume);
        }

        private IEnumerator SetVoumeLevel_Co(float value) {
            while (Mathf.Abs(m_VolumeMultiplier - value) > 0.01f) {
                m_VolumeMultiplier += Mathf.Sign(value - m_VolumeMultiplier) * Time.unscaledDeltaTime * m_LerpSpeed;
                m_AudioSource.volume = m_MusicLoop.Volume * m_GlobalMusicVolume * m_VolumeMultiplier;

                yield return null;
            }

            m_VolumeMultiplier = value;
        }

        public void PlayMusic() {
            if (m_MusicLoop.SoundClip == null || m_MusicLoop.Volume == 0f || m_MusicLoop.Pitch == 0f)
                return;

            m_AudioSource.volume = m_MusicLoop.Volume * m_GlobalMusicVolume * m_VolumeMultiplier;
            m_AudioSource.pitch = m_MusicLoop.Pitch;
            m_AudioSource.clip = m_MusicLoop.SoundClip;
            m_AudioSource.loop = true;
            m_AudioSource.time = m_MusicTime;
            m_AudioSource.Play();
        }

        public void PauseMusic() {
            m_MusicTime = m_AudioSource.time;
            m_AudioSource.Stop();
        }
    }
}