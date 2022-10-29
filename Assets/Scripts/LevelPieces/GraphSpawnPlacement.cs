using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
    [System.Serializable]
    public struct GraphSpawnPlacement
    {
        [HideLabel]
        public GraphArea Area;
        [BoxGroup]
        public WeightedListInt Rotation;
    }
}
