using Minipede.Gameplay;
using Minipede.Gameplay.Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
    public class EnemyControllerInstaller : MonoInstaller
    {
		[InlineEditor]
		[SerializeField] private EnemyBalances _balances;

		public override void InstallBindings()
		{
			Container.Bind<EnemyController>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			/* --- */

			Container.BindInterfacesTo<HealthBalanceResolver>()
				.AsSingle()
				.WithArguments( _balances.Health );

			Container.BindInterfacesTo<SpeedBalanceResolver>()
				.AsSingle()
				.WithArguments( _balances.Speed );
		}
	}
}
