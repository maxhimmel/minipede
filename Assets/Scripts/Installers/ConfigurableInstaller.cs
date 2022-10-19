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
				.AsTransient()
				.WithArguments( GetSettings() );
		}

		public virtual TSettings GetSettings()
		{
			return _settings;
		}
	}
}
