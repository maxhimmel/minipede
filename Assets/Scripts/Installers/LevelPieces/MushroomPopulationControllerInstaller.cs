using Minipede.Gameplay.Waves;
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

					subContainer.BindInterfacesAndSelfTo<TimedEnemySpawner>()
						.AsSingle()
						.WithArguments( _settings.ReplenishWave );
				} )
				.WithKernel()
				.AsSingle()
				.WithArguments( _settings );
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
					var populatorInstance = Container.TryResolve<MushroomPopulationController>();
					if ( populatorInstance != null )
					{
						var countVariable = typeof( MushroomPopulationController ).GetField(
							"_mushroomCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
						);
						mushroomCount = (int)countVariable.GetValue( populatorInstance );
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
