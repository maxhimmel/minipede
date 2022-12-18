using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class PoisonTrailFactory
	{
		private readonly PoisonVolume _settings;
		private readonly Lifetimer.Factory _lifetimerFactory;
		private readonly object[] _lifetimerArgs;

		public PoisonTrailFactory( PoisonVolume settings,
			Lifetimer.Factory lifetimerFactory )
		{
			_settings = settings;
			_lifetimerFactory = lifetimerFactory;
			_lifetimerArgs = new object[] { settings.Lifetime };
		}

		/// <summary>
		/// Creates a <see cref="Lifetimer"/> instance <b>and</b> starts the lifetime countdown.
		/// </summary>
		/// <param name="position"></param>
		public Lifetimer Create( Vector2 position )
		{
			var result = _lifetimerFactory.Create(
				_settings.Prefab,
				new Orientation( position ),
				_lifetimerArgs 
			);

			result.StartLifetime();
			return result;
		}
	}
}