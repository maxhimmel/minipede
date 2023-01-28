using System.Collections.Generic;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;

		private Camera _camera;
		private PlayerController _playerController;
		private MinimapMarkerFactoryBus _markerFactory;
		private Dictionary<Transform, MinimapMarker> _markers;

		[Inject]
		public void Construct( Camera camera,
			PlayerController playerController,
			MinimapMarkerFactoryBus markerFactory )
		{
			_camera = camera;
			_playerController = playerController;
			_markerFactory = markerFactory;

			_markers = new Dictionary<Transform, MinimapMarker>();
		}

		public void AddMarker( Transform avatar, MinimapMarker markerPrefab )
		{
			var newMarker = _markerFactory.Create( 
				markerPrefab, 
				new Orientation(avatar.position, GetMarkerDirection( avatar ), _container ) 
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
			var containerRect = _container.rect;

			foreach ( var avatar in _markers.Keys )
			{
				var avatarPos = avatar.position.ToVector2();
				var viewportPos = _camera.WorldToViewportPoint( avatarPos )
					.Clamp( Vector2.zero, Vector2.one );

				var marker = _markers[avatar];
				marker.transform.localPosition = new Vector2()
				{
					x = viewportPos.x * containerRect.width,
					y = viewportPos.y * containerRect.height
				};
				marker.transform.localRotation = GetMarkerDirection( avatar );
			}
		}

		private Quaternion GetMarkerDirection( Transform avatar )
		{
			return _container.InverseTransformDirection(
				avatar.position.ToVector2() - _playerController.Position
			).ToLookRotation();
		}
	}
}
