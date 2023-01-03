using Minipede.Cheats;
using Minipede.Gameplay.Enemies.Spawning;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Managers/CheatSettings" )]
    public class CheatSettings : ScriptableObjectInstaller
    {
		[HideLabel]
		[SerializeField] private CheatController.Settings _settings;

		public override void InstallBindings()
		{
			if ( _settings.UseWalletCheat )
			{
				LogCheatActivation<WalletCheat>();

				Container.BindInterfacesAndSelfTo<WalletCheat>()
					.AsSingle()
					.WithArguments( _settings.Wallet );
			}

			if ( _settings.UseMushroomShifterCheat )
			{
				LogCheatActivation<MushroomShifterCheat>();

				Container.BindInterfacesAndSelfTo<MushroomShifterCheat>()
					.AsSingle();
			}

			if ( _settings.DisableEnemies )
			{
				LogCheatActivation<EnemySpawnCheat>();

				Container.Decorate<EnemyWaveController>()
					.With<EnemySpawnCheat>()
					.AsSingle();
			}

			Container.BindInterfacesAndSelfTo<CheatController>()
				.AsSingle()
				.WithArguments( _settings );
		}

		private void LogCheatActivation<TCheat>()
		{
			Debug.LogWarning( $"<color=orange>[Cheat]</color> " +
				$"Initializing <b>{ typeof( TCheat ).Name.SplitPascalCase()}</b>." );
		}
	}
}
