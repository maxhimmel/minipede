using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
    public class ChildfreeSpawnPoint : MonoBehaviour,
        IOrientation
    {
        public Vector2 Position => transform.position;
		public Quaternion Rotation => transform.rotation;
		public Transform Parent => null;
    }
}
