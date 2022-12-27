using Sirenix.OdinInspector;

namespace Minipede.Cheats
{
	public class CheatController
	{
		[System.Serializable]
		public struct Settings
		{
			[ToggleGroup( "UseWalletCheat", "Wallet" )]
			public bool UseWalletCheat;
			[ToggleGroup( "UseWalletCheat" ), TableList( AlwaysExpanded = true ), HideLabel]
			public WalletCheat.Settings[] Wallet;
		}
	}
}