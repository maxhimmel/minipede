using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
    public class CleansedArea : MonoBehaviour
    {
		private Collider2D[] _colliders;
		private PollutedAreaController _pollutionController;

		[Inject]
		public void Construct( Collider2D[] colliders,
			PollutedAreaController pollutionController )
		{
			_colliders = colliders;
			_pollutionController = pollutionController;
		}

		public void Activate()
		{
			foreach ( var collider in _colliders )
			{
				collider.enabled = true;

				_pollutionController.Cleanse( new Bounds(
					center: collider.transform.position,
					size:	collider.transform.lossyScale
				) );
			}
		}

		public class Factory : UnityPrefabFactory<CleansedArea>
		{
			private readonly Transform _areaContainer;

			public Factory( DiContainer container )
				: base( container )
			{
				_areaContainer = container.ResolveId<Transform>( "CleansedAreaContainer" );
			}

			public override CleansedArea Create( Object prefab, IOrientation placement, IEnumerable<object> extraArgs = null )
			{
				var result = base.Create( prefab, placement, extraArgs );

				result.transform.SetParent( _areaContainer, worldPositionStays: true );

				return result;
			}
		}
	}
}
