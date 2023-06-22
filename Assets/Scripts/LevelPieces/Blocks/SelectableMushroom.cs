using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class SelectableMushroom : ISelectable
	{
		public IOrientation Orientation => _owner.ToData();

		private readonly Transform _owner;
		private readonly ExplorerController _explorerController;
		private readonly SelectableSpriteToggle _spriteToggle;
		private readonly ICameraToggler _cameraToggler;

		private Beacon _equippedBeacon;

		public SelectableMushroom( Transform owner,
			ExplorerController explorerController,
			SelectableSpriteToggle spriteToggle,
			ICameraToggler cameraToggler )
		{
			_owner = owner;
			_explorerController = explorerController;
			_spriteToggle = spriteToggle;
			_cameraToggler = cameraToggler;
		}

		public bool CanBeInteracted()
		{
			var explorer = _explorerController.Pawn;
			return explorer != null && explorer.TryGetFirstHaulable( out _equippedBeacon );
		}

		public void Select()
		{
			_spriteToggle.Select();
			_cameraToggler.Activate();
			_equippedBeacon.ShowCleansedAreaPreview( _owner.position );
		}

		public void Deselect()
		{
			_spriteToggle.Deselect();
			_cameraToggler.Deactivate();
			_explorerController.CameraToggler.Activate();

			_equippedBeacon.HideCleansedAreaPreview();
			_equippedBeacon = null;
		}
	}
}