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

		public Rigidbody2D Body => _body;
		public IOrientation Orientation => new Orientation( _body.position, _body.transform.rotation, _body.transform.parent );

		private IDamageController _damageController;
		private IMotor _motor;
		private IMaxSpeed _maxSpeed;
		private Rigidbody2D _body;
		private TreasureHauler _treasureHauler;

		private Vector2 _moveInput;
		private bool _isMoveInputConsumed;
		private bool _isCleanedUp;

		[Inject]
		public void Construct( IDamageController damageController,
			IMotor motor,
			IMaxSpeed maxSpeedSettings,
			Rigidbody2D body,
			TreasureHauler treasureHauler )
		{
            _damageController = damageController;
			_motor = motor;
			_maxSpeed = maxSpeedSettings;
			_body = body;
			_treasureHauler = treasureHauler;

			treasureHauler.HaulAmountChanged += TreasureHaulAmountChanged;
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

		private void TreasureHaulAmountChanged( float weight )
		{
			_maxSpeed.RestoreMaxSpeed();
			float currentMax = _maxSpeed.GetMaxSpeed();

			_maxSpeed.SetMaxSpeed( currentMax - weight );
		}

		public void CollectAllTreasure( Rigidbody2D collector )
		{
			_treasureHauler.CollectAll( collector );
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
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
			_treasureHauler.HaulAmountChanged -= TreasureHaulAmountChanged;

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
		}

		public class Factory : UnityFactory<Explorer> { }
	}
}
