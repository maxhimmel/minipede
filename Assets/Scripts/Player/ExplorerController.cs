using Minipede.Utility;
using Rewired;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class ExplorerController : IController<Explorer>
	{
		private readonly Rewired.Player _input;

		private Explorer _explorer;

		public ExplorerController( Rewired.Player input )
		{
			_input = input;
		}

		public void Possess( Explorer pawn )
		{
			_explorer = pawn;

			_input.AddAxisDelegate( OnMoveHorizontal, ReConsts.Action.Horizontal );
			_input.AddAxisDelegate( OnMoveVertical, ReConsts.Action.Vertical );
		}

		private void OnMoveHorizontal( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.right * data.GetAxis() );
		}

		private void OnMoveVertical( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.up * data.GetAxis() );
		}

		public void UnPossess()
		{
			_explorer = null;

			_input.RemoveInputEventDelegate( OnMoveHorizontal );
			_input.RemoveInputEventDelegate( OnMoveVertical );
		}
	}
}