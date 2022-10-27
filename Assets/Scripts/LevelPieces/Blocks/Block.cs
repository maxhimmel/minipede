using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public partial class Block : MonoBehaviour,
		IDamageable
	{
		private IDamageController _damageController;
		private SpriteRenderer _renderer;

		[Inject]
		public void Construct( IDamageController damageController,
			SpriteRenderer renderer )
		{
			_damageController = damageController;
			_renderer = renderer;

			damageController.Damaged += HandleDamageAnim;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		private void HandleDamageAnim( Rigidbody2D victimBody, HealthController health )
		{
			_renderer.color = _renderer.color.SetAlpha( health.Percentage );
			//_renderer.transform.localScale = Vector3.one * health.Percentage;
		}

		private void OnDestroy()
		{
			_damageController.Damaged -= HandleDamageAnim;
		}
	}
}
