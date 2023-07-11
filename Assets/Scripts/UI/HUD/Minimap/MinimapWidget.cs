using System.Collections.Generic;
using Minipede.Gameplay.Minimap;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public abstract class MinimapWidget : MonoBehaviour
	{
		[SerializeField] protected RectTransform _container;
		[SerializeField] private AnimationCurve _distanceFadeCurve = AnimationCurve.EaseInOut( 3, 1, 2, 0 );

		protected Camera _camera;
		protected MinimapModel _minimap;
		private MinimapMarkerFactoryBus _widgetFactory;

		protected readonly Dictionary<IMapMarker, MinimapMarker> _markers = new Dictionary<IMapMarker, MinimapMarker>();

		[Inject]
		public void Construct( Camera camera,
			MinimapModel minimap,
			MinimapMarkerFactoryBus markerFactory )
		{
			_camera = camera;
			_minimap = minimap;
			_widgetFactory = markerFactory;
		}

		private void OnDisable()
		{
			_minimap.MarkerAdded -= OnMarkerAdded;
			_minimap.MarkerRemoved -= OnMarkerRemoved;
		}

		private void OnEnable()
		{
			_minimap.MarkerAdded += OnMarkerAdded;
			_minimap.MarkerRemoved += OnMarkerRemoved;

			foreach ( var marker in _minimap.Markers )
			{
				if ( !_markers.ContainsKey( marker ) )
				{
					var newMarkerWidget = _widgetFactory.Create( marker.MarkerPrefab, marker.Avatar.ToData() );
					_markers.Add( marker, newMarkerWidget );
				}
			}
		}

		private void OnMarkerAdded( IMapMarker marker )
		{
			if ( !_markers.ContainsKey( marker ) )
			{
				_markers.Add( marker, _widgetFactory.Create( marker.MarkerPrefab, marker.Avatar.ToData() ) );
			}
		}

		private void OnMarkerRemoved( IMapMarker marker )
		{
			if ( _markers.Remove( marker, out var widget ) )
			{
				widget.Dispose();
			}
		}

		private void FixedUpdate()
		{
			if ( CanUpdate() )
			{
				UpdateMarkers();
			}
		}

		protected virtual bool CanUpdate()
		{
			return _markers.Count > 0;
		}

		protected abstract void UpdateMarkers();

		protected float GetMarkerAlpha( float distance )
		{
			return _distanceFadeCurve.Evaluate( distance );
		}
	}
}