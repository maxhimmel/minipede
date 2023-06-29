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

		private readonly static Collider2D[] _explosionBuffer = new Collider2D[30];

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
		private ISelectable _selector;
		private List<TargetGroupAttachment> _targetGroupAttachments;
		private ActionGlyphController _glyphController;
		private EquippedGunModel _equippedGunModel;
		private SignalBus _signalBus;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;
		private Beacon _equippedBeacon;
		private bool _isPiloted;
		private Gun _ejectExplosion;
		private Gun _repairExplosion;
		private Gun _defaultGun;

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
			ISelectable selector,
			List<TargetGroupAttachment> targetGroups,
			ActionGlyphController glyphController,
			EquippedGunModel equippedGunModel,
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
			_glyphController = glyphController;
			_equippedGunModel = equippedGunModel;
			_signalBus = signalBus;

			damageController.Died += OnDied;

			_ejectExplosion = _gunFactory.Create( settings.EjectExplosion );
			_ejectExplosion.SetOwner( transform );

			_repairExplosion = _gunFactory.Create( settings.RepairExplosion );
			_repairExplosion.SetOwner( transform );

			_defaultGun = _gunFactory.Create( settings.BaseGun );
			_defaultGun.SetOwner( transform );

			_equippedGunModel.SetGun( _defaultGun );
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

			_ejectExplosion.Reload();
			_ejectExplosion.StartFiring();

			TryKnockbackHaulables();

			_collider.enabled = false;

			_shrapnelFactory.Create( _body.position )
				.Launch( Random.insideUnitCircle.normalized * _settings.ShrapnelLaunchForce.Random() );

			await TaskHelpers.DelaySeconds( 0.15f, cancelToken );
			_collider.enabled = true;
		}

		private void TryKnockbackHaulables()
		{
			int overlapCount = Physics2D.OverlapCircleNonAlloc(
				_body.position,
				_settings.ExplosionRadius,
				_explosionBuffer,
				_settings.ExplosionLayer
			);

			for ( int idx = 0; idx < overlapCount; ++idx )
			{
				var collider = _explosionBuffer[idx];
				if ( collider.TryGetComponentFromBody<Haulable>( out var haulable ) )
				{
					haulable.Body.AddExplosionForce(
						_settings.ExplosionForce,
						_body.position,
						_settings.ExplosionRadius,
						_settings.ExplosionMode
					);
				}
			}
		}

		public void PossessedBy( ShipController controller )
		{
			_isPiloted = true;
			_renderer.color = Color.white;

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				targetGroupAttachment.Activate();
			}

			_minimap.RemoveMarker( transform );
		}

		public void UnPossess()
		{
			_isPiloted = false;

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
			_equippedGunModel.Gun.StartFiring();
		}

		public void StopFiring()
		{
			_equippedGunModel.Gun.StopFiring();
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
			_equippedGunModel.Gun.FixedTick();
			_ejectExplosion.FixedTick();
			_repairExplosion.FixedTick();
		}

		public bool Collect( ShipShrapnel shrapnel )
		{
			if ( !_isPiloted )
			{
				return false;
			}

			shrapnel.Dispose();

			Health.Replenish();

			_repairExplosion.Reload();
			_repairExplosion.StartFiring();

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

					var newGun = beacon.Gun;
					newGun.SetOwner( transform );
					newGun.Emptied += OnGunEmptied;

					_equippedGunModel.SetGun( newGun );

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
				var gun = _equippedGunModel.Gun;
				gun.StopFiring();
				gun.Emptied -= OnGunEmptied;
				_equippedGunModel.SetGun( _defaultGun );

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

			return _selector.CanBeInteracted();
		}

		public void Select()
		{
			_selector.Select();

			_glyphController.ShowAction( ReConsts.Action.Interact );
			if ( IsBeaconEquipped() )
			{
				_glyphController.ShowAction( ReConsts.Action.Fire );
			}
		}

		public void Deselect()
		{
			_selector.Deselect();
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

		public void PlaySpawnAnimation()
		{
			_signalBus.FireId( "Spawn", new FxSignal(
				transform.position,
				Vector2.up,
				transform
			) );

			PlayParkingAnimation();
		}

		public void PlayParkingAnimation()
		{
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );
		}

		public void SetActionGlyphsActive( bool isActive )
		{
			if ( isActive )
			{
				_glyphController.Activate();
			}
			else
			{
				_glyphController.Deactivate();
			}
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

			[BoxGroup( "Repair" )]
			public GunInstaller RepairExplosion;

			[BoxGroup( "Explosions" )]
			public LayerMask ExplosionLayer;
			[BoxGroup( "Explosions" )]
			public float ExplosionForce = 10;
			[BoxGroup( "Explosions" )]
			public float ExplosionRadius = 3;
			[BoxGroup( "Explosions" )]
			public ForceMode2D ExplosionMode = ForceMode2D.Impulse;
		}

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
