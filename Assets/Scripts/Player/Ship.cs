using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Ship : MonoBehaviour,
		IPawn,
		IDamageController
	{
		public event IDamageController.OnHit Damaged {
			add => _damageController.Damaged += value;
			remove => _damageController.Damaged -= value;
		}
		public event IDamageController.OnHit Died {
			add => _damageController.Died += value;
			remove => _damageController.Died -= value;
		}

		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );

		private IMotor _motor;
		private Gun _gun;
		private Rigidbody2D _body;
		private IDamageController _damageController;
		private PlayerController _playerSpawnController;
		private SpriteRenderer _renderer;

		private bool _isMoveInputConsumed;
		private Vector2 _moveInput;

		[Inject]
        public void Construct( IMotor motor,
			IDamageController damageController,
			Gun gun,
			Rigidbody2D body,
			PlayerController playerSpawnController,
			SpriteRenderer renderer )
		{
			_motor = motor;
			_damageController = damageController;
			_gun = gun;
			_body = body;
			_playerSpawnController = playerSpawnController;
			_renderer = renderer;

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

		public Explorer Eject()
		{
			_body.simulated = false;
			_renderer.color = new Color( 0.2f, 0.2f, 0.2f, 1 );

			return _playerSpawnController.CreateExplorer();
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

		public class Factory : PlaceholderFactory<Ship> { }
	}
}
