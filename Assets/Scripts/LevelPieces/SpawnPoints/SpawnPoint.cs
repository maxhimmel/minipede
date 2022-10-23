using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class SpawnPoint : MonoBehaviour,
		ISpawnPoint
	{
		public Vector2 Position => Container.position;
		public Quaternion Rotation => Container.rotation;
		public Transform Container => transform;
	}
}
