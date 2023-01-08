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
			Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _settings.InitialPoolSize )
					.FromSubContainerResolve()
					.ByNewContextPrefab( _settings.Projectile )
					.WithGameObjectName( _settings.Projectile.name )
				);
		}

		[System.Serializable]
		public struct Settings
		{
			public Gun.Settings Gun;
			[Space]
			public int InitialPoolSize;
			public GameObject Projectile;
			[Space]
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
