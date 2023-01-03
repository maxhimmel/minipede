using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class InteractableMushroom : IInteractable
	{
		public IOrientation Orientation => _owner.ToData();

		private readonly Transform _owner;
		private readonly ExplorerController _explorerController;

		public InteractableMushroom( Transform owner,
			ExplorerController explorerController )
		{
			_owner = owner;
			_explorerController = explorerController;
		}

		public bool CanBeInteracted()
		{
			return _explorerController.Pawn.TryGetHauledBeacon( out _ );
		}
	}
}