using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/StatusEffectReceiverInstaller" )]
	public class StatusEffectReceiverInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<StatusEffectController>()
				.AsSingle();
		}
	}
}