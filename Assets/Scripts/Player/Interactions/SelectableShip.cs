using Minipede.Gameplay.Cameras;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
	public class SelectableShip : ISelectable
	{
		public IOrientation Orientation => _owner.ToData();

		private readonly Transform _owner;
		private readonly ExplorerController _explorer;
		private readonly IDamageController _damageController;
		private readonly SpriteRenderer _selector;
		private readonly ICameraToggler _cameraToggler;

		public SelectableShip( Transform owner,
			ExplorerController explorer,
			IDamageController damageController,
			SpriteRenderer selector,
			ICameraToggler cameraToggler )
		{
			_owner = owner;
			_explorer = explorer;
			_damageController = damageController;
			_selector = selector;
			_cameraToggler = cameraToggler;
		}

		public bool CanBeInteracted()
		{
			if ( _damageController.Health.IsAlive )
			{
				return true;
			}

			return _explorer.Pawn.TryGetFirstHaulable( out ShipShrapnel shrapnel );
		}

		public void Select()
		{
			_selector.enabled = true;
			_cameraToggler.Activate();
		}

		public void Deselect()
		{
			_selector.enabled = false;
			_cameraToggler.Deactivate();
			_explorer.CameraToggler.Activate();
		}
	}
}