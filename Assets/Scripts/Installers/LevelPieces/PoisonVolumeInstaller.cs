using Minipede.Gameplay.Weapons;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Weapons/PoisonVolumeInstaller" )]
	public class PoisonVolumeInstaller : ScriptableObjectInstaller
	{
		[HideLabel]
		[SerializeField] private PoisonVolume _poison;

		public override void InstallBindings()
		{
			Container.Bind<LifetimerComponent.Factory>()
				.AsSingle();

			Container.BindInstances( 
				_poison,
				_poison.Trigger
			);
		}
	}
}