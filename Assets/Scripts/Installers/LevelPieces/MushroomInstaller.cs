using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class MushroomInstaller : MonoInstaller
	{
		[InlineEditor]
		[SerializeField] private MushroomBalances _balances;

		public override void InstallBindings()
		{
			Container.Bind<Rigidbody2D>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<Transform>()
				.FromComponentOnRoot()
				.AsSingle();

			Container.Bind<SpriteRenderer>()
				.FromMethod( GetComponentInChildren<SpriteRenderer> )
				.AsSingle();

			Container.Bind<Collider2D>()
				.FromMethod( GetComponentInChildren<Collider2D> )
				.AsSingle();

			/* --- */

			if ( _balances != null )
			{
				Container.BindInterfacesTo<HealthBalanceResolver>()
					.AsSingle()
					.WithArguments( _balances.Health );
			}
		}
	}
}