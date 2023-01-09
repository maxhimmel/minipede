using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class PoisonTrailInstaller : Installer<PoisonTrailInstaller.Settings, PoisonTrailInstaller>
	{
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
				);
		}

		[System.Serializable]
		public struct Settings
		{
			public int InitialPoolSize;
			public GameObject Prefab;
			public float Lifetime;
		}
	}
}