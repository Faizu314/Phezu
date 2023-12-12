using System;
using System.Collections.Generic;
using UnityEngine;

namespace Phezu.HyperCasualTemplate {

    [CreateAssetMenu(menuName = "Phezu/Hyper Casual Template/Levels", fileName = "GameLevels")]
    public class GameLevels : ScriptableObject {
        public List<LevelData> Levels;
    }

    [Serializable]
    public struct LevelData {
        public int StarsRequired;
    }
}