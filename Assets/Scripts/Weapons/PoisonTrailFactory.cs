using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class PoisonTrailFactory
	{
		private readonly PoisonVolume _settings;
		private readonly Lifetimer.Factory _lifetimerFactory;

		public PoisonTrailFactory( PoisonVolume settings,
			Lifetimer.Factory lifetimerFactory )
		{
			_settings = settings;
			_lifetimerFactory = lifetimerFactory;
		}

		/// <summary>
		/// Creates a <see cref="Lifetimer"/> instance <b>and</b> starts the lifetime countdown.
		/// </summary>
		/// <param name="position"></param>
		public Lifetimer Create( Vector2 position )
		{
			var result = _lifetimerFactory.Create(
				_settings.Prefab,
				new Orientation( position )
			);

			result.StartLifetime( _settings.Lifetime );
			return result;
		}
	}
}