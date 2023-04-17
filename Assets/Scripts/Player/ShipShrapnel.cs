using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.UI;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class ShipShrapnel : Collectable<ShipShrapnel>
	{
		private IMinimap _minimap;
		private MinimapMarker _markerPrefab;

		[Inject]
		public void Construct( IMinimap minimap,
			MinimapMarker markerPrefab )
		{
			_minimap = minimap;
			_markerPrefab = markerPrefab;
		}

		public override void Launch( Vector2 impulse )
		{
			base.Launch( impulse );
			_minimap.AddMarker( transform, _markerPrefab );
		}

		protected override void HandleDisposal()
		{
			_minimap.RemoveMarker( transform );
			base.HandleDisposal();
		}

		protected override ShipShrapnel GetCollectable()
		{
			return this;
		}

		public class Factory : UnityFactory<ShipShrapnel>
		{
			public Factory( DiContainer container, ShipShrapnel prefab ) 
				: base( container, prefab )
			{
			}
		}
	}
}
