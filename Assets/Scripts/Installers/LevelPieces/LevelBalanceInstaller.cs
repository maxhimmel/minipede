using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class LevelBalanceInstaller : MonoInstaller
	{
		[BoxGroup, HideLabel]
		[SerializeField] private LevelBalanceController.Settings _balancing;
		[BoxGroup, HideLabel]
		[SerializeField] private LevelCycleTimer.Settings _timing;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<LevelBalanceController>()
				.AsSingle()
				.WithArguments( _balancing );

			Container.BindInterfacesAndSelfTo<LevelCycleTimer>()
				.FromSubContainerResolve()
				.ByMethod( subContainer =>
				{
					subContainer.Bind<LevelCycleTimer>()
						.AsSingle();

					subContainer.BindInstance<LevelCycleTimer.ISettings>( _timing )
						.AsSingle();
				} )
				.AsSingle();

			/* --- */

			Container.DeclareSignal<LevelCycleChangedSignal>()
				.OptionalSubscriber();
		}
	}
}