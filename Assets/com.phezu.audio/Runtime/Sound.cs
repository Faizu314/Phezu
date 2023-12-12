using System;
using UnityEngine;

namespace Phezu.Audio {

    [Serializable]
    public struct Sound {
        public AudioClip SoundClip;
        [Range(0f, 1f)] public float Volume;
        [Range(0f, 1f)] public float Pitch;
    }
}