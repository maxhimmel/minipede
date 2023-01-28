using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class Minimap : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;

		[SerializeField] private bool _isRadial;

		private Camera _camera;
		private MinimapMarkerFactoryBus _markerFactory;
		private Dictionary<Transform, MinimapMarker> _markers;

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
				new Orientation(avatar.position, Quaternion.identity, _container ) 
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
			var radius = 0.5f * Mathf.Min( containerRect.width, containerRect.height );
			var center = new Vector2( containerRect.width * 0.5f, containerRect.height * 0.5f );

			foreach ( var avatar in _markers.Keys )
			{
				var avatarPos = avatar.position.ToVector2();
				var viewportPos = _camera.WorldToViewportPoint( avatarPos )
					.Clamp( Vector2.zero, Vector2.one );

				var markerPos = new Vector2()
				{
					x = viewportPos.x * containerRect.width,
					y = viewportPos.y * containerRect.height
				};

				if ( _isRadial )
				{
					var markerDir = markerPos - center;
					if ( markerDir.sqrMagnitude > radius * radius )
					{
						markerPos = center + markerDir.normalized * radius;
					}
				}

				var marker = _markers[avatar];
				marker.transform.localPosition = markerPos;
				marker.transform.localRotation = (markerPos - center).ToLookRotation();//GetMarkerDirection( avatar );
			}
		}

		//private Quaternion GetMarkerDirection( Transform avatar )
		//{
		//	return _container.InverseTransformDirection(
		//		avatar.position.ToVector2() - _playerController.Position
		//	).ToLookRotation();
		//}
	}
}
