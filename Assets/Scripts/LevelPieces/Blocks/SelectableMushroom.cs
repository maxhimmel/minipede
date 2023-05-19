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

		private Beacon _equippedBeacon;

		public SelectableMushroom( Transform owner,
			ExplorerController explorerController,
			SelectableSpriteToggle spriteToggle )
		{
			_owner = owner;
			_explorerController = explorerController;
			_spriteToggle = spriteToggle;
		}

		public bool CanBeInteracted()
		{
			return _explorerController.Pawn.TryGetFirstHaulable( out _equippedBeacon );
		}

		public void Select()
		{
			_spriteToggle.Select();
			_equippedBeacon.ShowCleansedAreaPreview( _owner.position );
		}

		public void Deselect()
		{
			_spriteToggle.Deselect();

			_equippedBeacon.HideCleansedAreaPreview();
			_equippedBeacon = null;
		}
	}
}