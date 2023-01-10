using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class PoisonMushroom : Mushroom
	{
		private PoisonTrailFactory _poisonTrailFactory;

		[Inject]
		public void Construct( PoisonTrailFactory poisonTrailFactory )
		{
			_poisonTrailFactory = poisonTrailFactory;
		}

		protected override void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			_poisonTrailFactory.Create( transform, transform.position );

			base.HandleDeath( victimBody, health );
		}

		public override void OnMoving()
		{
			base.OnMoving();

			_poisonTrailFactory.Create( transform, transform.position );
		}
	}
}
