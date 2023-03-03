using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class LevelBalanceInstaller : MonoInstaller
	{
		[BoxGroup( "Timing" ), HideLabel]
		[SerializeField] private LevelCycleTimer.Settings _timing;

		[BoxGroup( "Balancing" ), HideLabel]
		[SerializeField] private LevelBalanceController.Settings _balancing;

		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<LevelCycleTimer>()
				.AsSingle()
				.WithArguments( _timing );

			Container.BindInterfacesAndSelfTo<LevelBalanceController>()
				.AsSingle()
				.WithArguments( _balancing );

			/* --- */

			Container.DeclareSignal<LevelCycleChangedSignal>()
				.OptionalSubscriber();
		}
	}
}