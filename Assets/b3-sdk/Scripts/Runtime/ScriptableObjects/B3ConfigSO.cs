using UnityEngine;

namespace BasementSDK {
    [CreateAssetMenu(fileName = "B3 Config", menuName = "B3/B3 config", order = 1)]
    public class B3ConfigSO : ScriptableObject
    {
        public string gameSlug;
        [Tooltip("Is the game embedded on the basement launcher?")]
        public bool isWebGLGameEmbedded = true;
    }
}

