using Minipede.Gameplay.Cameras;
using Minipede.Utility;
using Rewired;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class ExplorerController : IController<Explorer>
	{
		public Explorer Pawn => _explorer;

		private readonly Rewired.Player _input;
		private readonly ICameraToggler<Explorer> _cameraToggler;
		private readonly InteractionHandlerBus<ExplorerController> _interactionHandler;

		private Explorer _explorer;

		public ExplorerController( Rewired.Player input,
			ICameraToggler<Explorer> cameraToggler,
			InteractionHandlerBus<ExplorerController> interactionHandler )
		{
			_input = input;
			_cameraToggler = cameraToggler;
			_interactionHandler = interactionHandler;
		}

		public void UnPossess()
		{
			_input.RemoveInputEventDelegate( OnInteract );
			_input.RemoveInputEventDelegate( OnStartGrabbing );
			_input.RemoveInputEventDelegate( OnStopGrabbing );
			_input.RemoveInputEventDelegate( OnStopReleasingTreasure );
			_input.RemoveInputEventDelegate( OnMoveHorizontal );
			_input.RemoveInputEventDelegate( OnMoveVertical );

			_cameraToggler.Deactivate( _explorer );

			_explorer.ReleaseAllTreasure();

			_explorer = null;
		}

		public void Possess( Explorer pawn )
		{
			_explorer = pawn;

			_input.AddButtonPressedDelegate( OnInteract, ReConsts.Action.Interact );
			_input.AddButtonPressedDelegate( OnStartGrabbing, ReConsts.Action.Fire );
			_input.AddButtonReleasedDelegate( OnStopGrabbing, ReConsts.Action.Fire );
			_input.AddButtonReleasedDelegate( OnStopReleasingTreasure, ReConsts.Action.Interact );
			_input.AddAxisDelegate( OnMoveHorizontal, ReConsts.Action.Horizontal );
			_input.AddAxisDelegate( OnMoveVertical, ReConsts.Action.Vertical );

			_cameraToggler.Activate( pawn );
		}

		private void OnInteract( InputActionEventData obj )
		{
			var interactable = _explorer.CurrentInteractable;
			if ( interactable != null )
			{
				_interactionHandler.Handle( this, interactable );
			}
			else
			{
				_explorer.StartReleasingTreasure();
			}
		}

		private void OnStartGrabbing( InputActionEventData obj )
		{
			if ( _explorer.CurrentInteractable is Ship ship )
			{
				ship.UnequipBeacon();
			}

			_explorer.StartGrabbing();
		}

		private void OnStopGrabbing( InputActionEventData obj )
		{
			_explorer.StopGrabbing();
		}

		private void OnStopReleasingTreasure( InputActionEventData obj )
		{
			_explorer?.StopReleasingTreasure();
		}

		private void OnMoveHorizontal( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.right * data.GetAxis() );
		}

		private void OnMoveVertical( InputActionEventData data )
		{
			_explorer.AddMoveInput( Vector2.up * data.GetAxis() );
		}
	}
}