using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.UI;
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
		private SpriteRenderer _renderer;
		private Collider2D _collider;
		private ISelectable _selectable;
		private IHealthBalanceResolver _healthBalancer;
		private ActionGlyphController _glyphController;
		private Vector3 _initialRendererScale;

		[Inject]
		public void Construct( Settings settings,
			LootBox lootBox,
			SpriteRenderer renderer,
			Collider2D collider,

			[InjectOptional] ISelectable selectable,
			[InjectOptional] IHealthBalanceResolver healthBalancer,
			[InjectOptional] ActionGlyphController glyphController )
		{
			_settings = settings;
			_lootBox = lootBox;
			_renderer = renderer;
			_collider = collider;

			_selectable = selectable;
			_healthBalancer = healthBalancer;
			_glyphController = glyphController;

			_initialRendererScale = _renderer.transform.localScale;
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
			if ( _selectable == null )
			{
				return false;
			}

			return _selectable.CanBeInteracted();
		}

		public void Select()
		{
			if ( _selectable != null )
			{
				_selectable.Select();
				_glyphController.ShowAction( ReConsts.Action.Interact );
			}
		}

		public void Deselect()
		{
			if ( _selectable != null )
			{
				_selectable.Deselect();
				_glyphController.HideAction( ReConsts.Action.Interact );
			}
		}

		public override void OnDespawned()
		{
			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			base.OnDespawned();
		}

		public override void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			RevertLighthouseConversion();

			_signalBus.Subscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			OnLevelCycleChanged();

			base.OnSpawned( placement, pool );
		}

		private void RevertLighthouseConversion()
		{
			_collider.enabled = true;

			// The "Convert" FX signal scales this down which is why we reset it here ...
			_renderer.transform.localScale = _initialRendererScale;
		}

		private void OnLevelCycleChanged()
		{
			_healthBalancer?.Resolve();
		}

		private void OnDestroy()
		{
			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
		}

		public void PrepareForLighthouseConversion()
		{
			_collider.enabled = false;
		}

		public void PlayConvertToLighthouseAnimation()
		{
			_signalBus.TryFireId( "Convert", new FxSignal(
				_body.position,
				Vector2.up,
				transform
			) );
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