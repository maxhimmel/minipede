using Minipede.Utility;
using Sirenix.OdinInspector;

namespace Minipede.Gameplay.LevelPieces
{
    [System.Serializable]
    public class GraphSpawnPlacement
    {
        [HideLabel]
        public GraphArea Area;
        [BoxGroup]
        public WeightedListInt Rotation;
    }
}
