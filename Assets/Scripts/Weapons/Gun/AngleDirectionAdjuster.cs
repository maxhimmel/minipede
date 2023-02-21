using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class AngleDirectionAdjuster : IDirectionAdjuster
	{
		[HideLabel]
		[SerializeField] private Settings _settings;

		public Vector2 Adjust( Vector2 direction )
		{
			float randAngle = _settings.AngleRange.Random() * RandomExtensions.Sign();
			return Quaternion.Euler( 0, 0, randAngle ) * direction;
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 180f, ShowFields = true )]
			public Vector2 AngleRange;
		}
	}
}
