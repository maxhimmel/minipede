using Minipede.Gameplay.LevelPieces;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Installers
{
	public class CleansedAreaInstaller : MonoInstaller
    {
		[SerializeField] private bool _startActivated;
		[SerializeField] private CleansedAreaAnimator.Settings _animator;

		public override void InstallBindings()
		{
			Container.Bind<Collider2D[]>()
				.FromMethod( GetComponentsInChildren<Collider2D> )
				.AsCached();

			// The starting area is already "activated" so it won't need to animate these things.
			if ( !_startActivated )
			{
				Container.Bind<SpriteRenderer[]>()
					.FromMethod( GetComponentsInChildren<SpriteRenderer> )
					.AsCached();

				Container.Bind<CleansedAreaAnimator>()
					.AsSingle()
					.WithArguments( _animator );
			}
		}
	}
}
