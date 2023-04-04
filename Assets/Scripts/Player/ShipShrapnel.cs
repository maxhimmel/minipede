using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class ShipShrapnel : Collectable<ShipShrapnel>
	{
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
