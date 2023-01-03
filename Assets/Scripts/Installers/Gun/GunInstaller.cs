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
				_settings.Projectile,
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
			Container.Bind<IProjectileProvider>()
				.To<ProjectileProvider>()
				.AsSingle();

			Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
				.FromFactory<Projectile.CustomFactory>();
		}

		[System.Serializable]
		public struct Settings
		{
			public Gun.Settings Gun;
			public ProjectileProvider.Settings Projectile;
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
