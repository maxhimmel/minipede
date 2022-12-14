using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu]
	public class StatusEffectReceiverInstaller : ScriptableObjectInstaller
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<StatusEffectController>()
				.AsSingle();
		}
	}
}