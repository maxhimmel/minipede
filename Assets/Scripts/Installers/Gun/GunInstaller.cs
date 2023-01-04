using System;
using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/Gun/Gun" )]
    public class GunInstaller : ScriptableObjectInstaller
    {
		[HideLabel]
		[SerializeField] private Settings _settings;

		public override void InstallBindings()
		{
			InstallModules();

			Container.BindInstances( 
				_settings.Gun, 
				_settings.Damage
			);

			BindProjectileFactory();

			Container.Bind<ShotSpot>()
				.AsSingle();

			Container.Bind<Gun>()
				.AsSingle();
		}

		private void InstallModules()
		{
			foreach ( var moduleInstaller in _settings.Modules )
			{
				Container.Inject( moduleInstaller );
				moduleInstaller.InstallBindings();
			}
		}

		private void BindProjectileFactory()
		{
			//Container.Bind<IProjectileProvider>()
			//	.To<ProjectileProvider>()
			//	.AsSingle()
			//	.WithArguments( _settings.Projectile );

			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromMonoPoolableMemoryPool( pool => pool
			//		.WithInitialSize( 30 )
			//		.FromSubContainerResolve()
			//		.ByNewContextPrefab( _settings.Projectile )
			//		//.ByNewPrefabInstaller<ProjectileInstaller>( _settings.Projectile )
			//		//.ByNewContextPrefab<ProjectileInstaller>( _settings.Projectile ) 
			//	);;

			// This works, too! With monobehaviours!
			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromSubContainerResolve()
			//	.ByNewContextPrefab<ProjectileInstaller>( _settings.Projectile );

			Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
				.FromMonoPoolableMemoryPool/*<Vector2, Quaternion, Projectile, Projectile.Pool>*/( pool => pool
					.WithInitialSize( 30 )
					.FromSubContainerResolve()
					.ByNewContextPrefab( _settings.Projectile )
				);

			// This works w/native c# style ...
			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromPoolableMemoryPool/*<Vector2, Quaternion, Projectile, Projectile.Pool>*/( pool => pool
			//		.WithInitialSize( 30 )
			//		.FromSubContainerResolve()
			//		.ByNewPrefabInstaller<ProjectileInstaller2>( _settings.Projectile )
			//	);

			// Close - BUT no cigar
			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromPoolableMemoryPool( pool =>
			//	{
			//		pool.WithInitialSize( 30 )
			//			.FromSubContainerResolve()
			//			.ByNewPrefabInstaller<ProjectileInstaller2>( _settings.Projectile );
			//	} );
			//.FromSubContainerResolve()
			//.ByNewPrefabInstaller<ProjectileInstaller2>( _settings.Projectile );

			// WE DID IT <3 <3 <3 <3
			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromSubContainerResolve()
			//	.ByNewPrefabInstaller<ProjectileInstaller2>( _settings.Projectile );

			// This works, too, but it's the old way.
			//Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
			//	.FromFactory<Projectile.CustomFactory>();

			// This works. Only difference? No gameobject context attached to prefab ...
			//Container.BindFactory<Vector2, Foobar, Foobar.Factory>()
			//	.FromMonoPoolableMemoryPool( pool =>
			//		pool.WithInitialSize( 30 )
			//		.FromComponentInNewPrefab( FoobarPrefab )
			//).NonLazy();
		}

		[System.Serializable]
		public struct Settings
		{
			public Gun.Settings Gun;
			//public ProjectileProvider.Settings Projectile;
			public GameObject Projectile;
			public DamageTrigger.Settings Damage;

			[Space, InlineEditor]
			public GunModuleInstaller[] Modules;
		}

#if UNITY_EDITOR
		private void OnValidate()
		{
			if ( string.IsNullOrEmpty( _settings.Gun.ShotSpotId ) )
			{
				_settings.Gun.ShotSpotId = "ShotSpot";
			}
		}
#endif
	}
}
