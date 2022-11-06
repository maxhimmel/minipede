using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public partial class Block : MonoBehaviour,
		IDamageController,
		ICleanup
	{
		public event IDamageController.OnHit Damaged;
		public event IDamageController.OnHit Died;
		//public event IDamageController.OnHit Damaged {
		//	add => _damageController.Damaged += value;
		//	remove => _damageController.Damaged -= value;
		//}
		//public event IDamageController.OnHit Died {
		//	add => _damageController.Died += value;
		//	remove => _damageController.Died -= value;
		//}

		private IDamageController _damageController;
		private SpriteRenderer _renderer;

		private bool _isCleanedUp;

		[Inject]
		public void Construct( /*IDamageController damageController,*/
			SpriteRenderer renderer )
		{
			//_damageController = damageController;
			_renderer = renderer;

			//damageController.Damaged += HandleDamageAnim;
			//damageController.Died += HandleDeath;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return 0;
			//return _damageController.TakeDamage( instigator, causer, data );
		}

		private void HandleDamageAnim( Rigidbody2D victimBody, HealthController health )
		{
			_renderer.color = _renderer.color.SetAlpha( health.Percentage );
			//_renderer.transform.localScale = Vector3.one * health.Percentage;
		}

		private void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			Cleanup();
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			//_damageController.Damaged -= HandleDamageAnim;
			//_damageController.Died -= HandleDeath;

			Destroy( gameObject );
			_isCleanedUp = true;
		}
	}
}
