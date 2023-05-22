using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class PoisonTrailFactory
	{
		private static Collider2D[] _queryBuffer = new Collider2D[5];

		private readonly Settings _settings;
		private readonly PoisonVolume.Factory _factory;
		private readonly float _lifetime;

		public PoisonTrailFactory( Settings settings,
			PoisonVolume.Factory factory,
			float lifetime )
		{
			_settings = settings;
			_factory = factory;
			_lifetime = lifetime;
		}

		/// <summary>
		/// Creates a <see cref="PoisonVolume"/> instance <b>and</b> starts the lifetime countdown.
		/// </summary>
		public PoisonVolume Create( Transform owner, Vector2 position )
		{
			if ( TryGetActivePoisonTrail( position, out var result ) )
			{
				result.SetOwner( owner );
				result.SetLifetime( _lifetime );
			}
			else
			{
				result = _factory.Create( owner, position, _lifetime );
			}

			result.StartExpiring();

			return result;
		}

		private bool TryGetActivePoisonTrail( Vector2 position, out PoisonVolume activePoison )
		{
			int queryCount = Physics2D.OverlapCircleNonAlloc(
				position,
				_settings.PoisonTrailRadius,
				_queryBuffer,
				_settings.PoisonTrailLayer
			);

			for ( int idx = 0; idx < queryCount; ++idx )
			{
				var result = _queryBuffer[idx];

				activePoison = result.GetComponentInParent<PoisonVolume>();
				if ( activePoison != null )
				{
					return true;
				}
			}

			activePoison = null;
			return false;
		}

		[System.Serializable]
		public class Settings
		{
			public LayerMask PoisonTrailLayer;
			public float PoisonTrailRadius = 0.5f;
		}
	}
}