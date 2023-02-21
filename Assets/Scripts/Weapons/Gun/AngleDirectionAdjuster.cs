using System;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class AngleDirectionAdjuster : IDirectionAdjuster
	{
		private readonly Settings _settings;

		public AngleDirectionAdjuster( Settings settings )
		{
			_settings = settings;
		}

		public Vector2 Adjust( Vector2 direction )
		{
			float randAngle = _settings.AngleRange.Random() * RandomExtensions.Sign();
			return Quaternion.Euler( 0, 0, randAngle ) * direction;
		}

		[System.Serializable]
		public struct Settings : IGunModule
		{
			public Type ModuleType => typeof( AngleDirectionAdjuster );

			[MinMaxSlider( 0, 180f, ShowFields = true )]
			public Vector2 AngleRange;
		}
	}
}
