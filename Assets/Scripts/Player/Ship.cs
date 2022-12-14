using System.Collections.Generic;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.Weapons;
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
		ISelectable
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
		private Gun _gun;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private PlayerController _playerController;
		private Inventory _inventory;
		private SpriteRenderer _renderer;
		private SpriteRenderer _selector;
		private TargetGroupAttachment _audioListenerTarget;
		private SignalBus _signalBus;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;
		private Beacon _equippedBeacon;
		private bool _isPiloted;

		[Inject]
        public void Construct( IMotor motor,
			IDamageController damageController,
			Gun gun,
			Rigidbody2D body,
			PlayerController playerController,
			Inventory inventory,
			SpriteRenderer renderer,
			[Inject( Id = "Selector" )] SpriteRenderer selector,
			List<TargetGroupAttachment> targetGroups,
			SignalBus signalBus )
		{
			_motor = motor;
			_damageController = damageController;
			_gun = gun;
			_body = body;
			_playerController = playerController;
			_inventory = inventory;
			_renderer = renderer;
			_selector = selector;
			_audioListenerTarget = targetGroups.Find( group => group.Id == "AudioListener" );
			_signalBus = signalBus;

			damageController.Died += OnDied;
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
			Destroy( gameObject );
		}

		public void PossessedBy( ShipController controller )
		{
			_isPiloted = true;
			_renderer.color = Color.white;
			_audioListenerTarget.enabled = true;
		}

		public void UnPossess()
		{
			_isPiloted = false;
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );
			_audioListenerTarget.enabled = false;

			_isMoveInputConsumed = true;
			_moveInput = Vector2.zero;
			_motor.SetDesiredVelocity( Vector2.zero );

			_playerController.CreateExplorer();
		}

		public void StartFiring()
		{
			_gun.StartFiring();
		}

		public void StopFiring()
		{
			_gun.StopFiring();
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
			_gun.FixedTick();
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

					return true;
				}
			}

			return false;
		}

		public void UnequipBeacon()
		{
			if ( IsBeaconEquipped() )
			{
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

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
