using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public abstract class Minimap : MonoBehaviour,
		IMinimap
	{
		[SerializeField] protected RectTransform _container;

		protected Camera _camera;
		private MinimapMarkerFactoryBus _markerFactory;
		protected Dictionary<Transform, MinimapMarker> _markers;

		[Inject]
		public void Construct( Camera camera,
			MinimapMarkerFactoryBus markerFactory )
		{
			_camera = camera;
			_markerFactory = markerFactory;

			_markers = new Dictionary<Transform, MinimapMarker>();
		}

		public void AddMarker( Transform avatar, MinimapMarker markerPrefab )
		{
			var newMarker = _markerFactory.Create(
				markerPrefab,
				new Orientation( avatar.position, Quaternion.identity, _container )
			);

			_markers.Add( avatar, newMarker );
		}

		public void RemoveMarker( Transform avatar )
		{
			if ( _markers.Remove( avatar, out var marker ) )
			{
				marker.Dispose();
			}
		}

		private void LateUpdate()
		{
			if ( CanUpdate() )
			{
				UpdateMap();
			}
		}

		protected virtual bool CanUpdate()
		{
			return _markers.Count > 0;
		}

		protected abstract void UpdateMap();
	}
}