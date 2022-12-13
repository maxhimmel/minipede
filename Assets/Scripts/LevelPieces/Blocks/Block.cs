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

		public HealthController Health => _damageController.Health;

		private Settings _settings;
		private IDamageController _damageController;
		private LootBox _lootBox;

		private bool _isCleanedUp;

		[Inject]
		public void Construct( Settings settings,
			IDamageController damageController,
			LootBox lootBox )
		{
			_settings = settings;
			_damageController = damageController;
			_lootBox = lootBox;

			damageController.Died += HandleDeath;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageType data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
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

			_damageController.Died -= HandleDeath;

			Destroy( gameObject );
			_isCleanedUp = true;
		}

		public async UniTask Heal()
		{
			while ( Health.Percentage < 1 )
			{
				int healAmount = TakeDamage( transform, transform, _settings.Heal );
				if ( healAmount != 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.DelayPerHealStep, this.GetCancellationTokenOnDestroy() )
						.SuppressCancellationThrow();
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[HideLabel]
			public HealDatum Heal;

			public float DelayPerHealStep;
		}
	}
}
