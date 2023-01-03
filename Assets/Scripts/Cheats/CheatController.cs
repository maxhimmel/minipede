using Sirenix.OdinInspector;

namespace Minipede.Cheats
{
	public class CheatController
	{
		[System.Serializable]
		public struct Settings
		{
			[ToggleGroup( "UseWalletCheat", "Wallet", CollapseOthersOnExpand = false )]
			public bool UseWalletCheat;
			[ToggleGroup( "UseWalletCheat" ), TableList( AlwaysExpanded = true ), HideLabel]
			public WalletCheat.Settings[] Wallet;

			[ToggleGroup( "UseMushroomShifterCheat", "Mushroom Shifter (use arrow keys)", CollapseOthersOnExpand = false )]
			public bool UseMushroomShifterCheat;

			[ToggleGroup( "DisableEnemies", "Disable Enemies", CollapseOthersOnExpand = false )]
			public bool DisableEnemies;
		}
	}
}