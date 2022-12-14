using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Movement;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class DragonflyController : EnemyController
	{
		private IRemoteMotor _motor;

		private float _createBlockChance;

		[Inject]
		public void Construct( Settings settings,
			IRemoteMotor motor )
		{
			_motor = motor;

			_createBlockChance = settings.CreateBlockRange.Random();
		}

		public override void StartMainBehavior()
		{
			base.StartMainBehavior();

			_motor.StartMoving( transform.up );
		}

		public override void RecalibrateVelocity()
		{
			_motor.RecalibrateVelocity();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( _levelForeman.TryQueryEmptyBlock( _body.position, out var instructions ) )
			{
				if ( _createBlockChance.DiceRoll() )
				{
					instructions.CreateStandardMushroom();
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[MinMaxSlider( 0, 1, ShowFields = true )]
			public Vector2 CreateBlockRange;

			[MinMaxSlider( 1, 10, ShowFields = true )]
			public Vector2Int ZigZagRange;
		}
	}
}