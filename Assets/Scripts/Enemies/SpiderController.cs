using Minipede.Gameplay.Movement;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class SpiderController : EnemyController
	{
		private IRemoteMotor _motor;

		[Inject]
		public void Construct( IRemoteMotor motor )
		{
			_motor = motor;
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( _motor.IsMoving && _levelForeman.TryQueryFilledBlock( _body.position, out var instructions ) )
			{
				// TODO: Implement a Kill() method for blocks ...
				instructions.Destroy();
			}
		}

		[System.Serializable]
		public struct Settings
		{

		}
	}
}