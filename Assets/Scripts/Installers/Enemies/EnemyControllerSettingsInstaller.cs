using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Installers
{
	public class EnemyControllerSettingsInstaller<TSettings> : EnemyControllerInstaller
	{
		[HideLabel]
		[SerializeField] private TSettings _settings;

		public override void InstallBindings()
		{
			base.InstallBindings();

			Container.BindInstance( _settings )
				.AsSingle();
		}
	}
}