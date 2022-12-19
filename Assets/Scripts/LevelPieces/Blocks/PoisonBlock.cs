using Minipede.Gameplay.Weapons;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class PoisonBlock : Block
	{
		private PoisonTrailFactory _poisonTrailFactory;

		[Inject]
		public void Construct( PoisonTrailFactory poisonTrailFactory )
		{
			_poisonTrailFactory = poisonTrailFactory;
		}

		public override void OnMoving()
		{
			base.OnMoving();

			_poisonTrailFactory.Create( transform.position );
		}
	}
}
