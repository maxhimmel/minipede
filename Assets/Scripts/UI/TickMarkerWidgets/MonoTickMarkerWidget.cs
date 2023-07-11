using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoTickMarkerWidget : MonoBehaviour,
		ITickMarkerWidget
	{
		[HorizontalGroup, ToggleLeft]
		[SerializeField] private bool _placeTicksOnStart;
		[HorizontalGroup, MinValue( 0 ), EnableIf( "_placeTicksOnStart" ), HideLabel]
		[SerializeField] private int _tickCount;

		[SerializeField] private SimplePlacement _placementPrefab;

		private IPlacement.Factory _placementFactory;

		[Inject]
		public void Construct( IPlacement.Factory placementFactory )
		{
			_placementFactory = placementFactory;
		}

		private void Start()
		{
			if ( _placeTicksOnStart )
			{
				PlaceTickMarkers( _tickCount );
			}
		}

		[Button]
		public void PlaceTickMarkers( int tickCount )
		{
			for ( int idx = 0; idx < tickCount; ++idx )
			{
				float progress = idx / (float)(tickCount - 1);
				PlaceTickMarker( progress );
			}
		}

		[Button]
		public void PlaceTickMarker( float progress )
		{
			CreateTickMarker()
				.Move( GetPosition( progress ) );
		}

		private IPlacement CreateTickMarker()
		{
			var newPlacement = _placementFactory.Create( _placementPrefab );

			if ( newPlacement is MonoBehaviour mono )
			{
				mono.transform.SetParent( transform, worldPositionStays: false );
			}

			return newPlacement;
		}

		protected abstract Vector2 GetPosition( float progress );
	}
}
