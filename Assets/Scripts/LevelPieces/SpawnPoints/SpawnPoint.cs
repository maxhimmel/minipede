using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class SpawnPoint : MonoBehaviour,
		IOrientation
	{
		public Vector2 Position => Parent.position;
		public Quaternion Rotation => Parent.rotation;
		public Transform Parent => transform;
	}
}
