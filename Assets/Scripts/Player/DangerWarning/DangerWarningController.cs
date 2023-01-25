using Minipede.Gameplay.Enemies;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class DangerWarningController : EnterExitTriggerFilter<EnemyController>
    {
		private IDangerWarningReaction[] _reactions;

		[Inject]
		public void Construct( IDangerWarningReaction[] reactions )
		{
			_reactions = reactions;
		}

		protected override void OnEntered( EnemyController other )
		{
			base.OnEntered( other );

			foreach ( var reaction in _reactions )
			{
				reaction.React( other );
			}
		}

		protected override void OnExited( EnemyController other )
		{
			base.OnExited( other );

			foreach ( var reaction in _reactions )
			{
				reaction.Neglect( other );
			}
		}
	}
}
