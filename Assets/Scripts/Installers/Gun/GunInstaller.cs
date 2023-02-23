using Minipede.Gameplay.Weapons;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class GunInstaller : MonoInstaller
    {
		[HideLabel]
		[SerializeField] private Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstances(
				_settings.Damage
			);

			Container.Bind<Gun>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<Gun>()
						.AsSingle()
						.WithArguments( _settings.Gun );

					subContainer.Bind<ShotSpot>()
						.FromSubContainerResolve()
						.ByMethod( subContainer =>
						{
							subContainer.Bind<ShotSpot>()
								.AsSingle();

							subContainer.Bind<Transform>()
								.FromResolveGetter<DiContainer>( container => container.ResolveId<Transform>( _settings.Gun.ShotSpotId ) )
								.AsSingle();
						} )
						.AsSingle();

					/* --- */

					subContainer.BindInterfacesTo( _settings.Gun.FireSpread.ModuleType )
						.AsSingle()
						.WithArguments( _settings.Gun.FireSpread );

					foreach ( var settings in _settings.Gun.Modules )
					{
						subContainer.BindInterfacesTo( settings.ModuleType )
							.AsCached()
							.WithArguments( settings );
					}

					/* --- */

					BindProjectileFactory( subContainer );
				} )
				.AsSingle();
		}

		private void BindProjectileFactory( DiContainer container )
		{
			container.BindFactory<float, Vector2, Quaternion, Projectile, Projectile.Factory>()
				.FromMonoPoolableMemoryPool( pool => pool
					.WithInitialSize( _settings.InitialPoolSize )
					.FromSubContainerResolve()
					.ByNewContextPrefab( _settings.Projectile )
					.WithGameObjectName( _settings.Projectile.name )
				);
		}

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Damage" ), HideLabel]
			public DamageTrigger.Settings Damage;

			[FoldoutGroup( "Projectile" )]
			public int InitialPoolSize;
			[FoldoutGroup( "Projectile" )]
			public GameObject Projectile;

			[FoldoutGroup( "Gun" ), HideLabel]
			public Gun.Settings Gun = new Gun.Settings();
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
