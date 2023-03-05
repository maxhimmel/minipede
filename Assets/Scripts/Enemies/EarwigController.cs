using Minipede.Gameplay.Movement;
using Minipede.Gameplay.Weapons;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class EarwigController : EnemyController
	{
		private IMotor _motor;
		private PoisonTrailFactory _poisonTrailFactory;

		[Inject]
		public void Construct( IMotor motor,
			PoisonTrailFactory poisonTrailFactory )
		{
			_motor = motor;
			_poisonTrailFactory = poisonTrailFactory;
		}

		public override void StartMainBehavior()
		{
			base.StartMainBehavior();

			_motor.SetDesiredVelocity( transform.up );
		}

		public override void RecalibrateVelocity()
		{
			base.RecalibrateVelocity();
			_motor.RecalibrateVelocity();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();

			if ( _levelForeman.TryQueryAnyBlock( _body.position, out var instructions ) )
			{
				if ( instructions.IsFilled )
				{
					if ( instructions.IsMushroom() && !instructions.IsPoisoned() )
					{
						instructions
							.Demolish()
							.Destroy()
							.CreatePoisonMushroom();
					}
				}
				else
				{
					var spawnPos = instructions.Cell.Center;
					_poisonTrailFactory.Create( transform, spawnPos );
				}
			}
		}
	}
}
