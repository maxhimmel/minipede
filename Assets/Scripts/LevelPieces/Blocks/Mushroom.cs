using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Mushroom : Block,
		ISelectable
	{
		public IOrientation Orientation => new Orientation( transform.position, transform.rotation, transform.parent );

		private Settings _settings;
		private LootBox _lootBox;
		private IInteractable _interactable;
		private ISelectable _selectable;
		private IHealthBalanceResolver _healthBalancer;

		[Inject]
		public void Construct( Settings settings,
			LootBox lootBox,
			[InjectOptional] IInteractable interactable,
			[InjectOptional] ISelectable selectable,
			[InjectOptional] IHealthBalanceResolver healthBalancer )
		{
			_settings = settings;
			_lootBox = lootBox;
			_interactable = interactable;
			_selectable = selectable;
			_healthBalancer = healthBalancer;
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

		public bool CanBeInteracted()
		{
			if ( _interactable == null )
			{
				return false;
			}

			return _interactable.CanBeInteracted();
		}

		public void Select()
		{
			_selectable?.Select();
		}

		public void Deselect()
		{
			_selectable?.Deselect();
		}

		public override void OnDespawned()
		{
			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			base.OnDespawned();
		}

		public override void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_signalBus.Subscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			OnLevelCycleChanged();

			base.OnSpawned( placement, pool );
		}

		private void OnLevelCycleChanged()
		{
			_healthBalancer?.Resolve();
		}

		[System.Serializable]
		public class Settings
		{
			[HideLabel]
			public HealInvoker.Settings Heal;
			public float DelayPerHealStep;
		}
	}
}