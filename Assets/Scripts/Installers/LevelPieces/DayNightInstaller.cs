using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class DayNightInstaller : MonoInstaller
	{
		[HideLabel]
		[SerializeField] private LevelBalanceController.Settings _levelBalance;

		[Space, HideLabel]
		[SerializeField] private DayNightModel.Settings _dayNight;

		public override void InstallBindings()
		{
			Container.Bind<LevelBalanceController>()
				.AsSingle()
				.WithArguments( _levelBalance );

			/* --- */

			Container.BindInstance( _dayNight )
				.AsSingle();

			Container.Bind<DayNightModel>()
				.AsSingle();

			Container.BindInterfacesAndSelfTo<DayNightController>()
				.AsSingle();

			/* --- */

			Container.DeclareSignal<LevelCycleChangedSignal>()
				.OptionalSubscriber();
		}
	}
}