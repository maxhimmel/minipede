using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
    public static class ZenjectExtensions
    {
        //public static ConditionCopyNonLazyBinder BindUnityFactory<TValue, TFactory>( this DiContainer self, TValue prefab )
        //    where TValue : Component
        //    where TFactory : UnityPlaceholderFactory<TValue>
        //{
        //    self.BindInstance( prefab )
        //        .WhenInjectedInto<UnityFactory<TValue>>();

        //    return self.BindFactory<Vector2, Quaternion, Transform, TValue, TFactory>()
        //        .FromFactory<UnityFactory<TValue>>();
        //}

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
	}

	public class UnityFactory<TValue> : IUnityFactory<TValue>
		where TValue : Component
	{
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
	}

	//public class UnityPlaceholderFactory<TValue> : PlaceholderFactory<Vector2, Quaternion, Transform, TValue>
	//{
	//}

	//public class UnityFactory<TPrefab> : IFactory<Vector2, Quaternion, Transform, TPrefab>
	//	where TPrefab : Component
	//{
	//	private readonly DiContainer _container;
	//	private readonly TPrefab _prefab;

	//	public UnityFactory( DiContainer container,
	//		TPrefab prefab )
	//	{
	//		_container = container;
	//		_prefab = prefab;
	//	}

	//	public virtual TPrefab Create( Vector2 param1, Quaternion param2, Transform param3 )
	//	{
	//		var result = _container.InstantiatePrefabForComponent<TPrefab>(
	//			_prefab,
	//			param1,
	//			param2,
	//			param3
	//		);

	//		result.name = _prefab.name;

	//		return result;
	//	}
	//}
}
