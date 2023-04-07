using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class InteractableShip : IInteractable
	{
		public IOrientation Orientation => _owner.ToData();

		private readonly Transform _owner;
		private readonly ExplorerController _explorer;
		private readonly IDamageController _damageController;

		public InteractableShip( Transform owner,
			ExplorerController explorer,
			IDamageController damageController )
		{
			_owner = owner;
			_explorer = explorer;
			_damageController = damageController;
		}

		public bool CanBeInteracted()
		{
			if ( _damageController.Health.IsAlive )
			{
				return true;
			}

			return _explorer.Pawn.TryGetFirstHaulable( out ShipShrapnel shrapnel );
		}
	}
}