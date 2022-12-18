using System.Collections.Generic;
using System.Linq;
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
		TValue Create( Vector2 position, IEnumerable<object> extraArgs = null );
		TValue Create( Vector2 position, Quaternion rotation, IEnumerable<object> extraArgs = null );
		TValue Create( Vector2 position, Quaternion rotation, Transform parent, IEnumerable<object> extraArgs = null );
		TValue Create( IOrientation placement, IEnumerable<object> extraArgs = null );
	}

	public class UnityFactory<TValue> : IUnityFactory<TValue>
		where TValue : Component
	{
		public TValue Prefab => _prefab;

		[Inject]
		private readonly TValue _prefab;
		[Inject]
		private readonly DiContainer _container;

		public TValue Create( Vector2 position, IEnumerable<object> extraArgs = null )
		{
			return Create( position, Quaternion.identity, extraArgs );
		}

		public TValue Create( Vector2 position, Quaternion rotation, IEnumerable<object> extraArgs = null )
		{
			return Create( position, rotation, null, extraArgs );
		}

		public TValue Create( Vector2 position, Quaternion rotation, Transform parent, IEnumerable<object> extraArgs = null )
		{
			if ( extraArgs == null )
			{
				extraArgs = Enumerable.Empty<object>();
			}

			var result = _container.InstantiatePrefabForComponent<TValue>( _prefab, position, rotation, parent, extraArgs );
			result.name = _prefab.name;

			if ( result.transform.parent != null && parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}

		public TValue Create( IOrientation placement, IEnumerable<object> extraArgs = null )
		{
			return Create( placement.Position, placement.Rotation, placement.Parent, extraArgs );
		}
	}

	public class UnityPrefabFactory<TValue> : IFactory<Object, IOrientation, IEnumerable<object>, TValue>
		where TValue : MonoBehaviour
	{
		[Inject]
		private readonly DiContainer _container;

		public TValue Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
		{
			if ( extraArgs == null )
			{
				extraArgs = Enumerable.Empty<object>();
			}

			var result = _container.InstantiatePrefabForComponent<TValue>( 
				prefab, 
				placement.Position, 
				placement.Rotation, 
				placement.Parent,
				extraArgs
			);
			result.name = prefab.name;

			if ( result.transform.parent != null && placement.Parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}
	}

	public class UnityPrefabFactory : IFactory<GameObject, IOrientation, GameObject>
	{
		[Inject]
		private readonly DiContainer _container;

		public GameObject Create( GameObject prefab, IOrientation placement )
		{
			var result = _container.InstantiatePrefab(
				prefab,
				placement.Position,
				placement.Rotation,
				placement.Parent
			);
			result.name = prefab.name;

			if ( result.transform.parent != null && placement.Parent == null )
			{
				result.transform.SetParent( null );
			}

			return result;
		}
	}
}
