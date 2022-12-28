using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Mushroom : Block
	{
		private Settings _settings;
		private LootBox _lootBox;

		[Inject]
		public void Construct( Settings settings,
			LootBox lootBox )
		{
			_settings = settings;
			_lootBox = lootBox;
		}

		protected override void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			base.HandleDeath( victimBody, health );

			_lootBox.Open( victimBody.position );
		}

		public async UniTask Heal()
		{
			while ( Health.Percentage < 1 )
			{
				// TODO: Fix null error breaking player from respawning:
				// Something about THIS component being null - probably accessing the transforms below is what's breaking.
				int healAmount = TakeDamage( transform, transform, _settings.Heal );
				if ( healAmount != 0 )
				{
					await TaskHelpers.DelaySeconds( _settings.DelayPerHealStep, this.GetCancellationTokenOnDestroy() )
						.SuppressCancellationThrow();
				}
			}
		}

		public virtual void OnMoving()
		{
		}

		[System.Serializable]
		public struct Settings
		{
			[HideLabel]
			public HealInvoker.Settings Heal;
			public float DelayPerHealStep;
		}
	}
}