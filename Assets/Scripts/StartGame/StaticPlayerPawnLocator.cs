using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class StaticPlayerPawnLocator : IPlayerPawnLocator
	{
		public IOrientation Orientation { get; }

		public StaticPlayerPawnLocator( [InjectOptional] Settings settings )
		{
			Orientation = settings != null ?
				new Orientation( settings.Position, Quaternion.Euler( settings.EulerRotation ), settings.Parent )
				: new Orientation();
		}

		[System.Serializable]
		public class Settings
		{
			public Vector2 Position;
			public Vector3 EulerRotation;
			public Transform Parent;
		}
	}
}