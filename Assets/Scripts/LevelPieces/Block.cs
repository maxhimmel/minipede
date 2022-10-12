using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Block : MonoBehaviour,
		IDamageable
	{
		private HealthController _health;
		private SpriteRenderer _renderer;

		[Inject]
		public void Construct( HealthController health,
			SpriteRenderer renderer )
		{
			_health = health;
			_renderer = renderer;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgDealt = _health.TakeDamage( data );
			Debug.LogFormat( data.LogFormat(), name, dmgDealt, instigator?.name, causer?.name );

			HandleDamageAnim();
			if ( !_health.IsAlive )
			{
				HandleDeath();
			}

			return dmgDealt;
		}

		private void HandleDamageAnim()
		{
			_renderer.transform.localScale = Vector3.one * _health.Percentage;
		}

		private void HandleDeath()
		{
			Destroy( gameObject );
		}

		public class Factory : PlaceholderFactory<Block> { }
	}
}
