using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EarwigController : EnemyController
	{
		private IMotor _motor;

		[Inject]
		public void Construct( IMotor motor )
		{
			_motor = motor;
		}

		protected override void OnReady()
		{
			base.OnReady();

			_motor.SetDesiredVelocity( transform.up );
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( _levelForeman.TryQueryFilledBlock( _body.position, out var instructions ) )
			{
				instructions
					.Destroy()
					.Create( Block.Type.Poison );
			}
		}

		public class Factory : PlaceholderFactory<EarwigController> { }
	}
}
