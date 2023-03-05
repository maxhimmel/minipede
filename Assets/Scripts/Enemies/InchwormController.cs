using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Movement;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
    public class InchwormController : EnemyController
    {
		private Settings _settings;
		private IMotor _motor;
		private EnemyDebuffController _debuffController;

		[Inject]
		public void Construct( Settings settings,
			IMotor motor,
			EnemyDebuffController debuffController )
		{
			_settings = settings;
			_motor = motor;
			_debuffController = debuffController;
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

		protected override void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			base.OnDied( victimBody, health );

			_debuffController.DebuffSpeed( _settings.SlowDownRatio, _settings.SlowDownDuration )
				.Forget();
		}

		protected override void FixedTick()
		{
			base.FixedTick();

			_motor.FixedTick();
		}

		[System.Serializable]
		public class Settings
		{
			[PropertyRange( 0, 1 )]
			public float SlowDownRatio;
			public float SlowDownDuration;
		}
	}
}
