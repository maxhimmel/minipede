using Minipede.Gameplay.Fx;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class PoolableFxSettings : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private PoolableFx.Settings _settings;

		public override void InstallBindings()
		{
			Container.BindInstance( _settings )
				.AsSingle();
		}
	}
}