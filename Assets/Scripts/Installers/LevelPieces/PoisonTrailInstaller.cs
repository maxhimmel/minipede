using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class PoisonTrailInstaller : Installer<PoisonTrailInstaller.Settings, PoisonTrailInstaller>
	{
		/// <summary>
		/// This ID should match a <see cref="Transform"/> within the scene being bound using a <see cref="ZenjectBinding"/>.
		/// </summary>
		private const string _containerId = "PoisonPool";

		[Inject]
		private readonly Settings _settings;

		public override void InstallBindings()
		{
			Container.Bind<PoisonTrailFactory>()
				.AsSingle()
				.WithArguments( _settings.Lifetime );

			Container.BindFactory<Transform, Vector3, float, PoisonVolume, PoisonVolume.Factory>()
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _settings.InitialPoolSize )
					.FromSubContainerResolve()
					.ByNewPrefabMethod( _settings.Prefab, container =>
						PoisonVolumeInstaller.Install( container )
					)
					.WithGameObjectName( _settings.Prefab.name )
					.UnderTransform( context => context
						.Container.ResolveId<Transform>( _containerId ) 
					)
				);
		}

		[System.Serializable]
		public class Settings
		{
			public int InitialPoolSize;
			public GameObject Prefab;
			public float Lifetime;
		}
	}
}