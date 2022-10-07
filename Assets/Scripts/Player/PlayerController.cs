using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Minipede.Installers;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
	{
		private Rewired.Player _input;
		private CharacterMotor _motor;
		private Gun _gun;

		[Inject]
        public void Construct( Rigidbody2D body,
            GameplaySettings.Player settings,
			Rewired.Player input,
			Gun gun )
		{
			_input = input;
			_motor = new CharacterMotor( body, settings.Movement );
			_gun = gun;
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
	}
}
