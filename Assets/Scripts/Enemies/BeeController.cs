using System.Collections;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class BeeController : EnemyController
	{
		private IMotor _motor;

		private float _createBlockChance;

		[Inject]
		public void Construct( Settings settings,
			IMotor motor )
		{
			_motor = motor;

			_createBlockChance = settings.CreateBlockRange.Random();
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

			if ( _levelForeman.TryQueryEmptyBlock( _body.position, out var instructions ) )
			{
				if ( _createBlockChance.DiceRoll() )
				{
					instructions.Create( Block.Type.Regular );
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 CreateBlockRange;
		}
	}
}
