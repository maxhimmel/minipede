using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
    public static class ZenjectExtensions
    {
		public static InstantiateCallbackConditionCopyNonLazyBinder BindUnityFactory<TValue, TFactory>( this DiContainer self, TValue prefab )
			where TValue : Component
			where TFactory : UnityFactory<TValue>
		{
			return self.Bind<TFactory>()
				.AsCached()
				.WithArguments( prefab );
		}
    }

	public interface IUnityFactory<TValue>
	{
		TValue Create( Vector2 position );
		TValue Create( Vector2 position, Quaternion rotation );
		TValue Create( Vector2 position, Quaternion rotation, Transform parent );
		TValue Create( IOrientation placement );
	}

	public class UnityFactory<TValue> : IUnityFactory<TValue>
		where TValue : Component
	{
		public TValue Prefab => _prefab;

		[Inject]
		private readonly TValue _prefab;
		[Inject]
		private readonly DiContainer _container;

		public TValue Create( Vector2 position )
		{
			return Create( position, Quaternion.identity );
		}

		public TValue Create( Vector2 position, Quaternion rotation )
		{
			return Create( position, rotation, null );
		}

		public TValue Create( Vector2 position, Quaternion rotation, Transform parent )
		{
			var result = _container.InstantiatePrefabForComponent<TValue>( _prefab, position, rotation, parent );
			result.name = _prefab.name;

			return result;
		}

		public TValue Create( IOrientation placement )
		{
			return Create( placement.Position, placement.Rotation, placement.Parent );
		}
	}
}
