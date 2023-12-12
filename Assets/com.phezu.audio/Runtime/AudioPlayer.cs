using System.Collections.Generic;
using UnityEngine;
using Phezu.Util;

namespace Phezu.Audio {

    public class AudioPlayer : Singleton<AudioPlayer> {
        [SerializeField] private List<AudioSource> m_AudioSources;

        private List<float> m_SourcesVolumes = new();
        private int m_CurrentSource;

        private float m_GlobalSoundVolume = 1f;
        public float GlobalSoundVolume {
            get {
                return m_GlobalSoundVolume;
            }
            set {
                m_GlobalSoundVolume = value;

                ApplyGlobalVolume();
            }
        }

        private void Awake() {
            for (int i = 0; i < m_AudioSources.Count; i++)
                m_SourcesVolumes.Add(0f);
        }

        private void ApplyGlobalVolume() {
            for (int i = 0; i < m_AudioSources.Count; i++) {
                m_AudioSources[i].volume = m_SourcesVolumes[i] * m_GlobalSoundVolume;
            }
        }

        public void PlaySound(Sound sound) {
            if (sound.SoundClip == null || sound.Volume == 0f || sound.Pitch == 0f)
                return;

            var source = m_AudioSources[m_CurrentSource];

            m_SourcesVolumes[m_CurrentSource] = sound.Volume;
            source.volume = sound.Volume * m_GlobalSoundVolume;
            source.pitch = sound.Pitch;
            source.clip = sound.SoundClip;
            source.loop = false;
            source.time = 0f;
            source.Play();

            m_CurrentSource = (m_CurrentSource + 1) % m_AudioSources.Count;
        }

        public int PlayLoop(Sound sound, float startTime) {
            if (sound.SoundClip == null || sound.Volume == 0f || sound.Pitch == 0f)
                return -1;

            var soundID = m_CurrentSource;

            var source = m_AudioSources[m_CurrentSource];

            m_SourcesVolumes[m_CurrentSource] = sound.Volume;
            source.volume = sound.Volume * m_GlobalSoundVolume;
            source.pitch = sound.Pitch;
            source.clip = sound.SoundClip;
            source.loop = true;
            source.time = startTime * sound.SoundClip.length;
            source.Play();

            m_CurrentSource = (m_CurrentSource + 1) % m_AudioSources.Count;

            return soundID;
        }

        public int PlayLoop(Sound sound) {
            if (sound.SoundClip == null || sound.Volume == 0f || sound.Pitch == 0f)
                return -1;

            var soundID = m_CurrentSource;

            var source = m_AudioSources[m_CurrentSource];

            m_SourcesVolumes[m_CurrentSource] = sound.Volume;
            source.volume = sound.Volume * m_GlobalSoundVolume;
            source.pitch = sound.Pitch;
            source.clip = sound.SoundClip;
            source.loop = true;
            source.Play();

            m_CurrentSource = (m_CurrentSource + 1) % m_AudioSources.Count;

            return soundID;
        }

        public void StopLoop(int soundID) {
            if (m_AudioSources[soundID] != null)
                m_AudioSources[soundID].Stop();
        }
    }
}
