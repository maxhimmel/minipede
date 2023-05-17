using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.UI;
using Minipede.Gameplay.Weapons;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Ship : MonoBehaviour,
		IPawn<Ship, ShipController>,
		IDamageController,
		ICollector<Treasure>,
		ICollector<Beacon>,
		ICollector<ShipShrapnel>,
		ISelectable,
		IPushable
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
		public Rigidbody2D Body => _body;
		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );

		private IMotor _motor;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private Settings _settings;
		private Inventory _inventory;
		private Gun.Factory _gunFactory;
		private ShipShrapnel.Factory _shrapnelFactory;
		private IMinimap _minimap;
		private Collider2D _collider;
		private SpriteRenderer _renderer;
		private SpriteRenderer _selector;
		private List<TargetGroupAttachment> _targetGroupAttachments;
		private IInteractable _interactable;
		private ActionGlyphController _glyphController;
		private SignalBus _signalBus;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;
		private Beacon _equippedBeacon;
		private bool _isPiloted;
		private Gun _ejectExplosion;
		private Gun _defaultGun;
		private Gun _equippedGun;

		[Inject]
        public void Construct( IMotor motor,
			IDamageController damageController,
			Settings settings,
			Rigidbody2D body,
			Inventory inventory,
			Gun.Factory gunFactory,
			ShipShrapnel.Factory shrapnelFactory,
			IMinimap minimap,
			Collider2D collider,
			SpriteRenderer renderer,
			[Inject( Id = "Selector" )] SpriteRenderer selector,
			List<TargetGroupAttachment> targetGroups,
			IInteractable interactable,
			ActionGlyphController glyphController,
			SignalBus signalBus )
		{
			_motor = motor;
			_damageController = damageController;
			_settings = settings;
			_body = body;
			_inventory = inventory;
			_gunFactory = gunFactory;
			_shrapnelFactory = shrapnelFactory;
			_minimap = minimap;
			_collider = collider;
			_renderer = renderer;
			_selector = selector;
			_targetGroupAttachments = targetGroups;
			_interactable = interactable;
			_glyphController = glyphController;
			_signalBus = signalBus;

			damageController.Died += OnDied;

			_ejectExplosion = _gunFactory.Create( settings.EjectExplosion );
			_ejectExplosion.SetOwner( transform );

			_defaultGun = _gunFactory.Create( settings.BaseGun );
			_defaultGun.SetOwner( transform );
			_equippedGun = _defaultGun;

			_signalBus.TryFire( _equippedGun.CreateEquippedSignal() );
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			if ( !_isPiloted )
			{
				return 0;
			}

			return _damageController.TakeDamage( instigator, causer, data );
		}

		private void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			UnequipBeacon();

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				targetGroupAttachment.Deactivate( canDispose: false ).Forget();
			}
		}

		public void AddMinimapMarker()
		{
			_minimap.AddMarker( transform, _settings.MapMarker );
		}

		public async UniTaskVoid Eject( Vector2 explorerPosition, CancellationToken cancelToken )
		{
			await TaskHelpers.DelaySeconds( _settings.ExplosionDelay, cancelToken );
			if ( cancelToken.IsCancellationRequested )
			{
				return;
			}

			var direction = (_body.position - explorerPosition).normalized;
			_signalBus.FireId( "Eject", new FxSignal( _body.position, direction, transform ) );

			_ejectExplosion.Reload();
			_ejectExplosion.StartFiring();

			_collider.enabled = false;

			_shrapnelFactory.Create( _body.position )
				.Launch( Random.insideUnitCircle.normalized * _settings.ShrapnelLaunchForce.Random() );

			await TaskHelpers.DelaySeconds( 0.15f, cancelToken );
			_collider.enabled = true;
		}

		public void PossessedBy( ShipController controller )
		{
			_isPiloted = true;
			_renderer.color = Color.white;

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				if ( !targetGroupAttachment.IsActive )
				{
					targetGroupAttachment.Activate();
				}
			}

			_minimap.RemoveMarker( transform );
		}

		public void UnPossess()
		{
			_isPiloted = false;
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				targetGroupAttachment.Deactivate( canDispose: false ).Forget();
			}

			_isMoveInputConsumed = true;
			_moveInput = Vector2.zero;
			_motor.SetDesiredVelocity( Vector2.zero );

			StopFiring();
		}

		public void StartFiring()
		{
			_equippedGun.StartFiring();
		}

		public void StopFiring()
		{
			_equippedGun.StopFiring();
		}

		public void AddMoveInput( Vector2 input )
		{
			_isMoveInputConsumed = false;
			_moveInput += input;
		}

		private void Update()
		{
			ConsumeDesiredVelocity();
		}

		private void ConsumeDesiredVelocity()
		{
			if ( _isMoveInputConsumed )
			{
				return;
			}

			_moveInput = Vector2.ClampMagnitude( _moveInput, 1 );
			_motor.SetDesiredVelocity( _moveInput );

			_moveInput = Vector2.zero;
			_isMoveInputConsumed = true;
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();
			_equippedGun.FixedTick();
			_ejectExplosion.FixedTick();
		}

		public bool Collect( ShipShrapnel shrapnel )
		{
			if ( !_isPiloted )
			{
				return false;
			}

			Health.Replenish();

			shrapnel.Dispose();
			return true;
		}

		public bool Collect( Treasure treasure )
		{
			if ( !_isPiloted )
			{
				return false;
			}

			_inventory.Collect( treasure.Resource, treasure.Body.position );
			treasure.Dispose();

			return true;
		}

		public bool Collect( Beacon beacon )
		{
			if ( _isPiloted )
			{
				beacon.StopFollowing();

				if ( !IsBeaconEquipped() )
				{
					beacon.Equip( _body );
					_equippedBeacon = beacon;

					_signalBus.TryFire( new BeaconEquippedSignal()
					{
						Beacon = beacon
					} );

					_equippedGun = beacon.Gun;
					_equippedGun.SetOwner( transform );
					_equippedGun.Emptied += OnGunEmptied;

					_signalBus.TryFire( _equippedGun.CreateEquippedSignal() );

					return true;
				}
			}

			return false;
		}

		private void OnGunEmptied( Gun gun, IAmmoHandler ammoHandler )
		{
			if ( IsBeaconEquipped() )
			{
				UnequipBeacon();
			}
		}

		public void UnequipBeacon()
		{
			if ( IsBeaconEquipped() )
			{
				_equippedGun.StopFiring();
				_equippedGun.Emptied -= OnGunEmptied;
				_equippedGun = _defaultGun;

				_signalBus.TryFire( _equippedGun.CreateEquippedSignal() );

				_equippedBeacon.Unequip();
				_equippedBeacon = null;

				_glyphController.HideAction( ReConsts.Action.Fire );

				_signalBus.TryFire( new BeaconUnequippedSignal() );
			}
		}

		private bool IsBeaconEquipped()
		{
			return _equippedBeacon != null;
		}

		public bool ToggleInventory()
		{
			return _inventory.ToggleVisibility();
		}

		public bool CanBeInteracted()
		{
			if ( _isPiloted )
			{
				return false;
			}

			return _interactable.CanBeInteracted();
		}

		public void Select()
		{
			_selector.enabled = true;

			_glyphController.ShowAction( ReConsts.Action.Interact );
			if ( IsBeaconEquipped() )
			{
				_glyphController.ShowAction( ReConsts.Action.Fire );
			}
		}

		public void Deselect()
		{
			_selector.enabled = false;
			_glyphController.HideAll();
		}

		public void SetCollisionActive( bool isActive )
		{
			int count = _body.attachedColliderCount;
			var colliders = new Collider2D[count];

			_body.GetAttachedColliders( colliders );

			for ( int idx = 0; idx < count; ++idx )
			{
				var collider = colliders[idx];
				collider.enabled = isActive;
			}
		}

		public void ClearMovement()
		{
			_motor.SetDesiredVelocity( Vector2.zero );

			_body.velocity = Vector2.zero;
			_body.angularVelocity = 0;
		}

		public void Push( Vector2 velocity )
		{
			_body.AddForce( velocity, ForceMode2D.Impulse );
		}

		[System.Serializable]
		public class Settings
		{
			public GunInstaller BaseGun;
			public MinimapMarker MapMarker;

			[BoxGroup( "Eject" )]
			public GunInstaller EjectExplosion;
			[BoxGroup( "Eject" ), MinValue( 0 )]
			public float ExplosionDelay;
			[BoxGroup( "Eject" )]
			public ShipShrapnel Shrapnel;
			[BoxGroup( "Eject" ), MinMaxSlider( 0, 100, ShowFields = true )]
			public Vector2 ShrapnelLaunchForce;
		}

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
