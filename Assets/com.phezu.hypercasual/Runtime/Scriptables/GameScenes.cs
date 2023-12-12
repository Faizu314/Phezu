using System;
using System.Collections.Generic;
using UnityEngine;
using Phezu.Util;

namespace Phezu.HyperCasualTemplate {

    [CreateAssetMenu(menuName = "Phezu/Hyper Casual Template/Scenes", fileName = "GameScenes")]
    public class GameScenes : ScriptableObject {

        [SceneField] public string MainMenuScene;
        [SceneField] public string LevelSelectionScene;
        [SceneField] public string OptionsScene;

        public List<LevelScene> LevelScenes;

        [Serializable]
        public struct LevelScene {
            [SceneField] public string SceneName;
        }
    }
}