using Minipede.Gameplay;
using Minipede.Gameplay.Weapons;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
    public class GunInstaller : ScriptableObjectInstaller
    {
		[SerializeField] private Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstances( 
				_settings.Gun, 
				_settings.Projectile,
				_settings.Damage
			);

			Container.Bind<ShotSpot>()
				.AsSingle();

			Container.Bind<IProjectileProvider>()
				.To<ProjectileProvider>()
				.AsSingle();

			Container.BindFactory<Vector2, Quaternion, Projectile, Projectile.Factory>()
				.FromFactory<Projectile.CustomFactory>();

			Container.Bind<Gun>()
				.AsSingle();
		}

		[System.Serializable]
		public struct Settings
		{
			public Gun.Settings Gun;
			public ProjectileProvider.Settings Projectile;
			public DamageTrigger.Settings Damage;
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
