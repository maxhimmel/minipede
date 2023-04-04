using System.Collections.Generic;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.UI;
using Minipede.Gameplay.Weapons;
using Minipede.Installers;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Ship : MonoBehaviour,
		IPawn<Ship, ShipController>,
		IDamageController,
		ICollector<Treasure>,
		ICollector<Beacon>,
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
		private PlayerController _playerController;
		private Inventory _inventory;
		private Gun.Factory _gunFactory;
		private IMinimap _minimap;
		private SpriteRenderer _renderer;
		private SpriteRenderer _selector;
		private TargetGroupAttachment _audioListenerTarget;
		private List<TargetGroupAttachment> _targetGroupAttachments;
		private SignalBus _signalBus;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;
		private Beacon _equippedBeacon;
		private bool _isPiloted;
		private Gun _defaultGun;
		private Gun _equippedGun;

		[Inject]
        public void Construct( IMotor motor,
			IDamageController damageController,
			Settings settings,
			Rigidbody2D body,
			PlayerController playerController,
			Inventory inventory,
			Gun.Factory gunFactory,
			IMinimap minimap,
			SpriteRenderer renderer,
			[Inject( Id = "Selector" )] SpriteRenderer selector,
			List<TargetGroupAttachment> targetGroups,
			SignalBus signalBus )
		{
			_motor = motor;
			_damageController = damageController;
			_settings = settings;
			_body = body;
			_playerController = playerController;
			_inventory = inventory;
			_gunFactory = gunFactory;
			_minimap = minimap;
			_renderer = renderer;
			_selector = selector;
			_audioListenerTarget = targetGroups.Find( group => group.Id == "AudioListener" );
			_targetGroupAttachments = targetGroups;
			_signalBus = signalBus;

			damageController.Died += OnDied;

			_defaultGun = _gunFactory.Create( settings.BaseGun );
			_defaultGun.SetOwner( transform );
			_equippedGun = _defaultGun;
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

			_damageController.Died -= OnDied;

			foreach ( var targetGroupAttachment in _targetGroupAttachments )
			{
				targetGroupAttachment.Deactivate( canDispose: true );
			}

			Destroy( gameObject );
		}

		public void PossessedBy( ShipController controller )
		{
			_isPiloted = true;
			_renderer.color = Color.white;

			if ( !_audioListenerTarget.IsActive )
			{
				_audioListenerTarget.Activate();
			}

			_minimap.RemoveMarker( transform );
		}

		public void UnPossess()
		{
			_isPiloted = false;
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );
			_audioListenerTarget.Deactivate( canDispose: false );

			_isMoveInputConsumed = true;
			_moveInput = Vector2.zero;
			_motor.SetDesiredVelocity( Vector2.zero );

			StopFiring();

			_playerController.CreateExplorer();

			_minimap.AddMarker( transform, _settings.MapMarker );
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
		}

		public bool Collect( Treasure treasure )
		{
			if ( !_isPiloted )
			{
				return false;
			}

			_inventory.Collect( treasure.Resource );
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

				_equippedBeacon.Unequip();
				_equippedBeacon = null;

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
			return !_isPiloted;
		}

		public void Select()
		{
			_selector.enabled = true;
		}

		public void Deselect()
		{
			_selector.enabled = false;
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
		}

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
