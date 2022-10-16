using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : MonoBehaviour,
		IDamageable
	{
		public IOnDestroyedNotify DestroyNotify => _destroyedNotify;

		private Rewired.Player _input;
		private IMotor _motor;
		private Gun _gun;
		private IOnDestroyedNotify _destroyedNotify;
		private IDamageController _damageController;

		[Inject]
        public void Construct( Rewired.Player input,
            IMotor motor,
			IDamageController damageController,
			Gun gun,
			IOnDestroyedNotify destroyedNotify )
		{
			_input = input;
			_motor = motor;
			_damageController = damageController;
			_gun = gun;
			_destroyedNotify = destroyedNotify;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		private void Update()
		{
			HandleMovement();
			HandleGun();
		}

		private void HandleMovement()
		{
			var moveInput = _input.GetAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical );
			moveInput = Vector2.ClampMagnitude( moveInput, 1 );

			_motor.SetDesiredVelocity( moveInput );
		}

		private void HandleGun()
		{
			if ( _input.GetButtonDown( ReConsts.Action.Fire ) )
			{
				_gun.StartFiring();
			}
			else if ( _input.GetButtonUp( ReConsts.Action.Fire ) )
			{
				_gun.StopFiring();
			}
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();
			_gun.FixedTick();
		}

		public class Factory : PlaceholderFactory<PlayerController> { }
	}
}
