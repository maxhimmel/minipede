using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ConfigurableInstaller<TConfigurable, TSettings> : MonoInstaller
	{
		[SerializeField] protected TSettings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TConfigurable>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<TSettings>()
				.FromInstance( GetSettings() )
				.AsSingle();
		}

		public virtual TSettings GetSettings()
		{
			return _settings;
		}
	}
}
