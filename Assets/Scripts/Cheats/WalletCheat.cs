using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class WalletCheat : IInitializable
	{
		private readonly Settings[] _settings;
		private readonly Inventory _inventory;

		public WalletCheat( Settings[] settings,
			Inventory inventory )
		{
			_settings = settings;
			_inventory = inventory;
		}

		public void Initialize()
		{
			Debug.LogWarning( $"<color=orange>[Cheat]</color> " +
				$"Initializing wallet cheat." );

			foreach ( var setting in _settings )
			{
				for ( int idx = 0; idx < setting.PreLoad; ++idx )
				{
					_inventory.Collect( setting.ResourceType );
				}
			}
		}

		[System.Serializable]
		public struct Settings
		{
			[Tooltip( "Amount of resources to pre-load into the player's wallet." )]
			public int PreLoad;
			public ResourceType ResourceType;
		}
	}
}