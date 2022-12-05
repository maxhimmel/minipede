using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class ConfigurableInstaller<TConfigurable, TSettings> : ScriptableObjectInstaller
	{
		[SerializeField] protected TSettings _settings;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<TConfigurable>()
				.AsSingle();
			//.WithArguments( GetSettings() );

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
