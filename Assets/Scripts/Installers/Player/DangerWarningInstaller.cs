using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class DangerWarningInstaller : MonoInstaller
	{
		[SerializeField] private LayerMask _dangerFilter = -1;
		[SerializeReference] private IDangerWarningReaction.ISettings[] _reactions;

		public override void InstallBindings()
		{
			Container.BindInstance( _dangerFilter )
				.AsSingle()
				.WhenInjectedInto<DangerWarningController>();

			Container.Bind<DangerWarningController>()
				.FromResolveGetter<Transform>( owner => owner.GetComponentInChildren<DangerWarningController>() )
				.AsSingle();

			/* --- */

			foreach ( var reaction in _reactions )
			{
				var installer = Container.InstantiateExplicit(
					reaction.InstallerType,
					InjectUtil.CreateArgListExplicit( reaction )
				) as IInstaller;

				installer.InstallBindings();
			}
		}
	}


	/* --- */


	public class DangerWarningReactionInstaller<TReaction> :
		Installer<IDangerWarningReaction.ISettings, DangerWarningReactionInstaller<TReaction>>
		where TReaction : IDangerWarningReaction
	{
		[Inject]
		private IDangerWarningReaction.ISettings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TReaction>()
				.AsCached()
				.WithArguments( _settings );
		}

		protected TSettings GetSettings<TSettings>()
			where TSettings : IDangerWarningReaction.ISettings
		{
			return (TSettings)_settings;
		}
	}
}