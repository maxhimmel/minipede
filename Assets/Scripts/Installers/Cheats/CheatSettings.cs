using System.Text;
using Minipede.Cheats;
using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.StartSequence;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/CheatSettings" )]
    public class CheatSettings : ScriptableObjectInstaller
    {
		[SerializeField] private bool _enableCheats = false;

		[HideLabel, ShowIf( "_enableCheats" )]
		[SerializeField] private Settings _settings;

		public override void InstallBindings()
		{
			if ( !_enableCheats )
			{
				return;
			}

			var messageBuilder = new StringBuilder($"<color=orange>CHEATS ACTIVATED</color>\n" +
				$"<i>(select this for details)</i>\n" );

			if ( _settings.UseWalletCheat )
			{
				LogCheatActivation<WalletCheat>( messageBuilder );

				Container.BindInterfacesAndSelfTo<WalletCheat>()
					.AsSingle()
					.WithArguments( _settings.Wallet );
			}

			if ( _settings.UseMushroomShifterCheat )
			{
				LogCheatActivation<MushroomShifterCheat>( messageBuilder );

				Container.BindInterfacesAndSelfTo<MushroomShifterCheat>()
					.AsSingle();
			}

			if ( _settings.DisableWaves )
			{
				LogCheatActivation<WaveControllerCheat>( messageBuilder );

				Container.Decorate<IWaveController>()
					.With<WaveControllerCheat>()
					.AsSingle();
			}

			if ( _settings.DisableLevelGeneration )
			{
				LogCheatActivation<LevelGeneratorCheat>( messageBuilder );

				Container.Decorate<LevelGenerator>()
					.With<LevelGeneratorCheat>()
					.AsSingle();
			}

			if ( _settings.UseFakeWinPercentage )
			{
				LogCheatActivation<LevelWonResolverCheat>( messageBuilder );

				Container.Decorate<IPollutionWinPercentage>()
					.With<LevelWonResolverCheat>()
					.AsSingle()
					.WithArguments( _settings.LevelWonResolver );
			}

			if ( _settings.IsShipGod )
			{
				LogCheatActivation<ShipGodModeCheat>( messageBuilder );

				Container.BindInterfacesAndSelfTo<ShipGodModeCheat>()
					.AsSingle();
			}

			if ( _settings.UseLevelCycleCheat )
			{
				LogCheatActivation<LevelCycleCheat>( messageBuilder );

				Container.Decorate<LevelCycleTimer.ISettings>()
					.With<LevelCycleCheat>()
					.WithArguments( _settings.LevelCycle );
			}

			if ( _settings.UseLevelBalanceCheat )
			{
				LogCheatActivation<LevelBalanceCheat>( messageBuilder );

				Container.Decorate<LevelBalanceController>()
					.With<LevelBalanceCheat>()
					.WithArguments( _settings.LevelBalance );
			}

			if ( _settings.CanSkipStartingSequence )
			{
				LogCheatActivation<LevelBalanceCheat>( messageBuilder );

				Container.Decorate<ILevelStartSequence>()
					.With<LevelStartSequenceSkipCheat>();
			}

			if ( _settings.UseKeyboardActions )
			{
				LogCheatActivation<KeyboardCheats>( messageBuilder );

				Container.BindInterfacesAndSelfTo<KeyboardCheats>()
					.AsSingle()
					.WithArguments( _settings.KeyboardActions );
			}

			Debug.LogWarning( messageBuilder );
		}

		private void LogCheatActivation<TCheat>( StringBuilder messageBuilder )
		{
			messageBuilder.AppendLine( $"<color=orange>[Cheat]</color> " +
				$"Initializing <b>{ typeof( TCheat ).Name.SplitPascalCase()}</b>." );
		}

		[System.Serializable]
		public class Settings
		{
			[ToggleGroup( "UseWalletCheat", "Wallet", CollapseOthersOnExpand = false )]
			public bool UseWalletCheat;
			[ToggleGroup( "UseWalletCheat", CollapseOthersOnExpand = false ), TableList( AlwaysExpanded = true ), HideLabel]
			public WalletCheat.Settings[] Wallet;

			[ToggleGroup( "UseMushroomShifterCheat", "Mushroom Shifter (use arrow keys)", CollapseOthersOnExpand = false )]
			public bool UseMushroomShifterCheat;

			[ToggleGroup( "DisableWaves", "Disable Waves", CollapseOthersOnExpand = false )]
			public bool DisableWaves;

			[ToggleGroup( "DisableLevelGeneration", "Disable Level Generation", CollapseOthersOnExpand = false )]
			public bool DisableLevelGeneration;

			[ToggleGroup( "UseFakeWinPercentage", "Fake Pollution Win Percentage", CollapseOthersOnExpand = false )]
			public bool UseFakeWinPercentage;
			[ToggleGroup( "UseFakeWinPercentage", CollapseOthersOnExpand = false ), HideLabel]
			public LevelWonResolverCheat.Settings LevelWonResolver;

			[ToggleGroup( "IsShipGod", "God Mode: Ship", CollapseOthersOnExpand = false )]
			public bool IsShipGod;

			[ToggleGroup( "UseLevelCycleCheat", "Level Cycle Timing", CollapseOthersOnExpand = false )]
			public bool UseLevelCycleCheat;
			[ToggleGroup( "UseLevelCycleCheat", CollapseOthersOnExpand = false ), HideLabel]
			public LevelCycleCheat.Settings LevelCycle;

			[ToggleGroup( "UseLevelBalanceCheat", "Level Balancer", CollapseOthersOnExpand = false )]
			public bool UseLevelBalanceCheat;
			[ToggleGroup( "UseLevelBalanceCheat", CollapseOthersOnExpand = false ), HideLabel]
			public LevelBalanceCheat.Settings LevelBalance;

			[ToggleGroup( "CanSkipStartingSequence", "Skip Level-Start Sequence", CollapseOthersOnExpand = false )]
			public bool CanSkipStartingSequence;

			[ToggleGroup( "UseKeyboardActions", "Keyboard Actions", CollapseOthersOnExpand = false )]
			public bool UseKeyboardActions;
			[ToggleGroup( "UseKeyboardActions", CollapseOthersOnExpand = false ), HideLabel]
			[SerializeReference, HideReferenceObjectPicker, LabelText( "Actions" )]
			public KeyboardCheats.Action[] KeyboardActions = new KeyboardCheats.Action[0];
		}
	}
}
