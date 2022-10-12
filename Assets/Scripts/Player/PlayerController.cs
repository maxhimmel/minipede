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
		private Rewired.Player _input;
		private CharacterMotor _motor;
		private HealthController _health;
		private Gun _gun;

		[Inject]
        public void Construct( Rigidbody2D body,
            GameplaySettings.Player settings,
			Rewired.Player input,
			Gun gun )
		{
			_input = input;
			
			_motor = new CharacterMotor( body, settings.Movement );
			_health = new HealthController( settings.Health );
			
			_gun = gun;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			int dmgDealt = _health.TakeDamage( data );
			Debug.LogFormat( data.LogFormat(), name, dmgDealt, instigator?.name, causer?.name );

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
