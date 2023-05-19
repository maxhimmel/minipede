using Minipede.Installers;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class MushroomPopulationControllerInstaller : MonoInstaller
    {
		[HideLabel]
		[SerializeField] private MushroomPopulationController.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<MushroomPopulationController>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<MushroomPopulationController>()
						.AsSingle()
						.WithArguments( _settings );

					TimedEnemySpawnerInstaller.Install( subContainer, _settings.ReplenishWave );
				} )
				.WithKernel()
				.AsSingle();
		}


		/* --- Editor/Tooling --- */


#if UNITY_EDITOR
		[OnInspectorGUI]
		private void DebugInfo()
		{
			using ( new UnityEditor.EditorGUI.DisabledGroupScope( true ) )
			{
				int mushroomCount = 0;

				if ( Application.isPlaying )
				{
					var activeBlocks = Container.TryResolve<ActiveBlocks>();
					if ( activeBlocks != null )
					{
						mushroomCount = activeBlocks.Actives.Count;
					}
				}

				UnityEditor.EditorGUILayout.Space();
				UnityEditor.EditorGUILayout.LabelField( "Debugging", UnityEditor.EditorStyles.boldLabel );
				UnityEditor.EditorGUILayout.IntField( "Mushroom Count", mushroomCount );
			}
		}
#endif
	}
}
