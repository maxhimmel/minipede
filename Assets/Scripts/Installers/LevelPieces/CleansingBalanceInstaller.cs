using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CleansingBalanceInstaller : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private CleansingBalanceController.Settings _balance;

		public override void InstallBindings()
		{
			Container.BindInstance( _balance )
				.AsSingle();

			Container.BindInterfacesTo<CleansingBalanceController>()
				.AsSingle();
		}
	}
}