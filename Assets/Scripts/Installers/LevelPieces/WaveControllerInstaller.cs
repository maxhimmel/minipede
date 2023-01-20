using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Waves/WaveController" )]
	public class WaveControllerInstaller : ScriptableObjectInstaller
	{
		[BoxGroup( "Main" ), HideLabel]
		[SerializeField] private WaveController.Settings _settings;

		[FoldoutGroup( "Enemies" ), HideLabel]
		[SerializeField] private EnemyWaveController.Settings _enemy;
		[FoldoutGroup( "Enemies" ), LabelText( "Waves" )]
		[SerializeField] private EnemyWaveInstaller[] _enemyWaves;
		[FoldoutGroup( "Enemies/Spider Spawning" ), HideLabel]
		[SerializeField] private SpiderSpawnController.Settings _spider;

		[FoldoutGroup( "Gathering" ), HideLabel]
		[SerializeField] private GatheringWaveController.Settings _gathering;

		public override void InstallBindings()
		{
			DeclareSignals();

			/* --- */

			Container.Bind<WaveController>()
				.AsSingle()
				.WithArguments( _settings );

			/* --- */

			// Lower priority wave controllers come first ...
			BindEnemyWaveModules();

			// Higher priority wave controllers come last ...
			Container.BindInterfacesTo<GatheringWaveController>()
				.AsSingle()
				.WithArguments( _gathering );
		}

		private void DeclareSignals()
		{
			Container.DeclareSignal<WaveQueueChangedSignal>()
				.OptionalSubscriber();

			Container.DeclareSignal<WaveProgressSignal>()
				.OptionalSubscriber();
		}

		private void BindEnemyWaveModules()
		{
			Container.BindInterfacesTo<EnemyWaveController>()
				.AsSingle()
				.WithArguments( _enemy );

			for ( int idx = 0; idx < _enemyWaves.Length; ++idx )
			{
				var waveInstaller = _enemyWaves[idx];

				Container.Inject( waveInstaller );
				waveInstaller.InstallBindings();
			}

			Container.BindInterfacesAndSelfTo<SpiderSpawnController>()
				.AsSingle()
				.WithArguments( _spider );
		}
	}
}