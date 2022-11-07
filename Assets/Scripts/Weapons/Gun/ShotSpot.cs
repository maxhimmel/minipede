using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class ShotSpot
	{
		public Vector2 Position => _shotSpot.position;
		public Vector2 Facing => Rotation * Vector2.up;
		public Vector2 Tangent => Rotation * Vector2.right;
		public Quaternion Rotation => _shotSpot.rotation;

		private readonly Transform _shotSpot;

		public ShotSpot( DiContainer container,
			Gun.Settings settings )
		{
			_shotSpot = container.ResolveId<Transform>( settings.ShotSpotId );
		}
	}
}
