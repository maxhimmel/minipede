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

			[ToggleGroup( "DisableLevelGeneration", "Disable Level Generation", CollapseOthersOnExpand = false )]
			public bool DisableLevelGeneration;

			[ToggleGroup( "UseFakeWinPercentage", "Fake Pollution Win Percentage", CollapseOthersOnExpand = false )]
			public bool UseFakeWinPercentage;
			[ToggleGroup( "UseFakeWinPercentage" ), HideLabel]
			public LevelWonResolverCheat.Settings LevelWonResolver;
		}
	}
}