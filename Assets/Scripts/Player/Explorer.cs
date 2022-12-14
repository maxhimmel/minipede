using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Explorer : MonoBehaviour,
		IPawn,
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
		public Rigidbody2D Body => _body;
		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );

		private IDamageController _damageController;
		private IMotor _motor;
		private Rigidbody2D _body;
		private TreasureHauler _treasureHauler;

		private Vector2 _moveInput;
		private bool _isMoveInputConsumed;
		private bool _isCleanedUp;

		[Inject]
		public void Construct( IDamageController damageController,
			IMotor motor,
			Rigidbody2D body,
			TreasureHauler treasureHauler )
		{
            _damageController = damageController;
			_motor = motor;
			_body = body;
			_treasureHauler = treasureHauler;

			damageController.Died += OnDied;
		}

		public void EnterShip()
		{
			Cleanup();
		}

		public void StartGrabbing()
		{
			_treasureHauler.StartGrabbing();
		}

		public void StopGrabbing()
		{
			_treasureHauler.StopGrabbing();
		}

		public void ReleaseAllTreasure()
		{
			_treasureHauler.ReleaseAll();
		}

		public void StartReleasingTreasure()
		{
			_treasureHauler.StartReleasingTreasure();
		}

		public void StopReleasingTreasure()
		{
			_treasureHauler.StopReleasingTreasure();
		}

		public void CollectAllTreasure( Rigidbody2D collector )
		{
			_treasureHauler.CollectAll( collector );
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageType data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		public int TakeDamage<TDamage, TSettings>( Transform instigator, Transform causer, TSettings data )
			where TDamage : IDamageType<TSettings>
		{
			return _damageController.TakeDamage<TDamage, TSettings>( instigator, causer, data );
		}

		private void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			Cleanup();
		}

		public void Cleanup()
		{
			if ( _isCleanedUp )
			{
				return;
			}

			_damageController.Died -= OnDied;

			Destroy( gameObject );
			_isCleanedUp = true;
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

			RotateTowardsVelocity();
		}

		private void RotateTowardsVelocity()
		{
			Vector2 velocity = _motor.Velocity;
			if ( velocity.sqrMagnitude >= 0.01f )
			{
				_body.MoveRotation( velocity.ToLookRotation() );
			}
		}

		public class Factory : UnityFactory<Explorer> { }
	}
}
