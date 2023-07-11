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
			Container.Bind<Gun>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<Gun>()
						.AsSingle()
						.WithArguments( _settings.Gun );

					subContainer.BindInstance( _settings.Damage );

					subContainer.Bind<ShotSpot>()
						.FromSubContainerResolve()
						.ByMethod( subContainer =>
						{
							subContainer.Bind<ShotSpot>()
								.AsSingle();

							subContainer.BindInstance( _settings.Gun.ShotSpot );
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
				} )
				.AsSingle();
		}

		[System.Serializable]
		public class Settings
		{
			[FoldoutGroup( "Damage" ), HideLabel]
			public DamageTrigger.Settings Damage;

			[FoldoutGroup( "Gun" ), HideLabel]
			public Gun.Settings Gun = new Gun.Settings();
		}
	}
}
