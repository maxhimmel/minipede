using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public partial class Block : MonoBehaviour,
		IDamageController,
		ICleanup
	{
		public event IDamageController.OnHit Damaged {
			add => _damageController.Damaged += value;
			remove => _damageController.Damaged -= value;
		}
		public event IDamageController.OnHit Died {
			add => _damageController.Died += value;
			remove => _damageController.Died -= value;
		}

		private Settings _settings;
		private IDamageController _damageController;
		private SpriteRenderer _renderer;
		private LootBox _lootBox;

		private bool _isCleanedUp;

		[Inject]
		public void Construct( Settings settings,
			IDamageController damageController,
			SpriteRenderer renderer,
			LootBox lootBox )
		{
			_settings = settings;
			_damageController = damageController;
			_renderer = renderer;
			_lootBox = lootBox;

			damageController.Damaged += HandleDamageAnim;
			damageController.Died += HandleDeath;
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

		private void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			_lootBox.Open( victimBody.position );
			Cleanup();
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			_damageController.Damaged -= HandleDamageAnim;
			_damageController.Died -= HandleDeath;

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		public async UniTask Heal()
		{
			int healAmount;
			do
			{
				healAmount = TakeDamage( transform, transform, new DamageDatum( _settings.HealStep ) );
				if ( healAmount != 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.DelayPerHealStep, this.GetCancellationTokenOnDestroy() )
						.SuppressCancellationThrow();
				}

			} while ( healAmount != 0 );
		}

		[System.Serializable]
		public struct Settings
		{
			[MaxValue( -1 )]
			public int HealStep;
			public float DelayPerHealStep;
		}
	}
}
