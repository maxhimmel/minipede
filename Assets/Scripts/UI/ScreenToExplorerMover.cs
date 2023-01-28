using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class ScreenToExplorerMover : MonoBehaviour
    {
        private ExplorerController _explorerController;
		private Camera _camera;

		[Inject]
		public void Construct( ExplorerController explorerController,
			Camera camera )
		{
            _explorerController = explorerController;
			_camera = camera;
		}

		private void LateUpdate()
		{
			if ( !CanMove() )
			{
				return;
			}

			var explorerPos = _explorerController.Pawn.Body.position;
			var viewportPos = _camera.WorldToViewportPoint( explorerPos );

			transform.position = new Vector2( viewportPos.x * Screen.width, viewportPos.y * Screen.height );
		}

		private bool CanMove()
		{
			return _explorerController.Pawn != null;
		}
	}
}
