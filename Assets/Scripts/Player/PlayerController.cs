using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Minipede.Installers;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : MonoBehaviour,
		IDamageable
	{
		public IOnDestroyedNotify DestroyNotify => _destroyedNotify;

		private Rewired.Player _input;
		private HealthController _health;
		private IMotor _motor;
		private Gun _gun;
		private IOnDestroyedNotify _destroyedNotify;

		[Inject]
        public void Construct( Rewired.Player input,
			HealthController health,
            IMotor motor,
			Gun gun,
			IOnDestroyedNotify destroyedNotify )
		{
			_input = input;
			_health = health;
			_motor = motor;
			_gun = gun;
			_destroyedNotify = destroyedNotify;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgDealt = _health.TakeDamage( data );
			Debug.LogFormat( data.LogFormat(), name, dmgDealt, instigator?.name, causer?.name );

			if ( !_health.IsAlive )
			{
				Destroy( gameObject );
			}

			return dmgDealt;
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
