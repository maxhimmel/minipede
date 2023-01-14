using System.Text;
using Minipede.Cheats;
using Minipede.Gameplay;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
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

			if ( _settings.DisableEnemies )
			{
				LogCheatActivation<EnemySpawnCheat>( messageBuilder );

				Container.Decorate<EnemyWaveController>()
					.With<EnemySpawnCheat>()
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

			Debug.LogWarning( messageBuilder );
		}

		private void LogCheatActivation<TCheat>( StringBuilder messageBuilder )
		{
			messageBuilder.AppendLine( $"<color=orange>[Cheat]</color> " +
				$"Initializing <b>{ typeof( TCheat ).Name.SplitPascalCase()}</b>." );
		}

		[System.Serializable]
		public struct Settings
		{
			[ToggleGroup( "UseWalletCheat", "Wallet", CollapseOthersOnExpand = false )]
			public bool UseWalletCheat;
			[ToggleGroup( "UseWalletCheat", CollapseOthersOnExpand = false ), TableList( AlwaysExpanded = true ), HideLabel]
			public WalletCheat.Settings[] Wallet;

			[ToggleGroup( "UseMushroomShifterCheat", "Mushroom Shifter (use arrow keys)", CollapseOthersOnExpand = false )]
			public bool UseMushroomShifterCheat;

			[ToggleGroup( "DisableEnemies", "Disable Enemies", CollapseOthersOnExpand = false )]
			public bool DisableEnemies;

			[ToggleGroup( "DisableLevelGeneration", "Disable Level Generation", CollapseOthersOnExpand = false )]
			public bool DisableLevelGeneration;

			[ToggleGroup( "UseFakeWinPercentage", "Fake Pollution Win Percentage", CollapseOthersOnExpand = false )]
			public bool UseFakeWinPercentage;
			[ToggleGroup( "UseFakeWinPercentage", CollapseOthersOnExpand = false ), HideLabel]
			public LevelWonResolverCheat.Settings LevelWonResolver;
		}
	}
}
