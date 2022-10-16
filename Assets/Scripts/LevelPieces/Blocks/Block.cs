using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public partial class Block : MonoBehaviour,
		IDamageable
	{
		private Damageable _damageable;
		private SpriteRenderer _renderer;

		[Inject]
		public void Construct( Damageable damageable,
			SpriteRenderer renderer )
		{
			_damageable = damageable;
			_renderer = renderer;

			damageable.Damaged += HandleDamageAnim;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageable.TakeDamage( instigator, causer, data );
		}

		private void HandleDamageAnim( object sender, HealthController health )
		{
			_renderer.transform.localScale = Vector3.one * health.Percentage;
		}

		private void OnDestroy()
		{
			_damageable.Damaged -= HandleDamageAnim;
		}
	}
}
