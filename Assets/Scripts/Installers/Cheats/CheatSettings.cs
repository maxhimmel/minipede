using Minipede.Cheats;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    [CreateAssetMenu]
    public class CheatSettings : ScriptableObjectInstaller
    {
		[HideLabel]
		[SerializeField] private CheatController.Settings _settings;

		public override void InstallBindings()
		{
			if ( _settings.UseWalletCheat )
			{
				Container.BindInterfacesAndSelfTo<WalletCheat>()
					.AsSingle()
					.WithArguments( _settings.Wallet );
			}

			Container.BindInterfacesAndSelfTo<CheatController>()
				.AsSingle()
				.WithArguments( _settings );
		}
	}
}
