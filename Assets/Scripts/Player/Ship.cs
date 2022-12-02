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
		ICollector
	{
		public event IDamageController.OnHit Damaged {
			add => _damageController.Damaged += value;
			remove => _damageController.Damaged -= value;
		}
		public event IDamageController.OnHit Died {
			add => _damageController.Died += value;
			remove => _damageController.Died -= value;
		}

		public Rigidbody2D Body => _body;
		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );

		private IMotor _motor;
		private Gun _gun;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private PlayerController _playerSpawnController;
		private SpriteRenderer _renderer;
		private TargetGroupAttachment _audioListenerTarget;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;

		[Inject]
        public void Construct( IMotor motor,
			IDamageController damageController,
			Gun gun,
			Rigidbody2D body,
			PlayerController playerSpawnController,
			SpriteRenderer renderer,
			List<TargetGroupAttachment> targetGroups )
		{
			_motor = motor;
			_damageController = damageController;
			_gun = gun;
			_body = body;
			_playerSpawnController = playerSpawnController;
			_renderer = renderer;
			_audioListenerTarget = targetGroups.Find( group => group.Id == "AudioListener" );

			damageController.Died += OnDied;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		private void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			_damageController.Died -= OnDied;
			Destroy( gameObject );
		}

		public void PossessedBy( ShipController controller )
		{
			_body.simulated = true;
			_renderer.color = Color.white;
			_audioListenerTarget.enabled = true;
		}

		public void UnPossess()
		{
			_body.simulated = false;
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );
			_audioListenerTarget.enabled = false;

			_isMoveInputConsumed = true;
			_moveInput = Vector2.zero;
			_motor.SetDesiredVelocity( Vector2.zero );

			_playerSpawnController.CreateExplorer();
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

		public void Collect( Treasure treasure )
		{
			treasure.Cleanup();
		}

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
