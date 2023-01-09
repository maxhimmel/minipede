using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class PoisonTrailFactory
	{
		private readonly PoisonVolume.Factory _factory;
		private readonly float _lifetime;

		public PoisonTrailFactory( PoisonVolume.Factory factory,
			float lifetime )
		{
			_factory = factory;
			_lifetime = lifetime;
		}

		/// <summary>
		/// Creates a <see cref="PoisonVolume"/> instance <b>and</b> starts the lifetime countdown.
		/// </summary>
		public PoisonVolume Create( Transform owner, Vector2 position )
		{
			var result = _factory.Create( owner, position, _lifetime );
			result.StartExpiring();

			return result;
		}
	}
}