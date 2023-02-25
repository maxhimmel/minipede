using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class WaveControllerInstaller : MonoInstaller
	{
		[BoxGroup( "Main" ), HideLabel]
		[SerializeField] private WaveController.Settings _settings;

		[FoldoutGroup( "Enemies" ), HideLabel]
		[SerializeField] private EnemyWaveController.Settings _enemy;
		[FoldoutGroup( "Enemies" ), LabelText( "Waves" )]
		[SerializeField] private EnemyWaveInstaller[] _enemyWaves;
		[FoldoutGroup( "Enemies/Spider Spawning" ), HideLabel]
		[SerializeField] private TimedEnemySpawner.Settings _spider;

		[FoldoutGroup( "Gathering" ), HideLabel]
		[SerializeField] private GatheringWaveController.Settings _gathering;

		public override void InstallBindings()
		{
			DeclareSignals();

			/* --- */

			Container.BindInterfacesAndSelfTo<WaveController>()
				.AsSingle()
				.WithArguments( _settings );

			/* --- */

			// Lower priority wave controllers come first ...
			BindEnemyWaveModules();

			// Higher priority wave controllers come last ...
			BindGatheringWaveModules();
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

			Container.BindInterfacesAndSelfTo<TimedEnemySpawner>()
				.AsSingle()
				.WithArguments( _spider );
		}

		private void BindGatheringWaveModules()
		{
			Container.BindInterfacesTo<GatheringWaveController>()
				.AsSingle()
				.WithArguments( _gathering );

			Container.Bind<GatheringWave>()
				.AsSingle()
				.WithArguments( _gathering.Wave );
		}
	}
}