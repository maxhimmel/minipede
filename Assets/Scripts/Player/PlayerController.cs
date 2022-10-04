using Minipede.Gameplay.Movement;
using Minipede.Installers;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : MonoBehaviour
	{
		private Rewired.Player _input;
		private CharacterMotor _motor;

        [Inject]
        public void Construct( Rigidbody2D body,
            GameplaySettings.Player settings,
			Rewired.Player input )
		{
			_input = input;
			_motor = new CharacterMotor( body, settings.Movement );
		}

		private void Update()
		{
			HandleMovement();
		}

		private void HandleMovement()
		{
			var moveInput = _input.GetAxis2D( ReConsts.Action.Horizontal, ReConsts.Action.Vertical );
			moveInput = Vector2.ClampMagnitude( moveInput, 1 );

			_motor.SetDesiredVelocity( moveInput );
		}

		private void FixedUpdate()
		{
			_motor.FixedTick();
		}
	}
}
