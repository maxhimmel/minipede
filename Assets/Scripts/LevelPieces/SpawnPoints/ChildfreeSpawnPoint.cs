using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
    public class ChildfreeSpawnPoint : MonoBehaviour,
        ISpawnPoint
    {
        public Vector2 Position => transform.position;
		public Quaternion Rotation => transform.rotation;
		public Transform Container => null;
    }
}
