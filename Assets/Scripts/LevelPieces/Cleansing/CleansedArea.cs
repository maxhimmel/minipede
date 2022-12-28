using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class CleansedArea : MonoBehaviour
    {
		private List<Collider2D> _colliders;

		[Inject]
		public void Construct( List<Collider2D> colliders )
		{
			_colliders = colliders;
		}

		public void Activate()
		{
			foreach ( var collider in _colliders )
			{
				collider.enabled = true;
			}

			// TODO: What about the sprites?
			// ...
		}

		public void Deactivate()
		{
			foreach ( var collider in _colliders )
			{
				collider.enabled = false;
			}

			// TODO: What about the sprites?
			// ...
		}

		public class Factory : UnityPrefabFactory<CleansedArea>
		{
			[Inject( Id = "CleansedAreaContainer" )]
			private readonly Transform _areaContainer;

			public override CleansedArea Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
			{
				var result = base.Create( prefab, placement, extraArgs );

				result.transform.SetParent( _areaContainer, worldPositionStays: true );

				return result;
			}
		}
	}
}
