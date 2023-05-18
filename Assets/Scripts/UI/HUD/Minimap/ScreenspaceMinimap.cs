using Minipede.Utility;
using Sirenix.Utilities;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ScreenspaceMinimap : Minimap
    {
		[SerializeField] private bool _isRadial;

		protected override void UpdateMap()
		{
			var containerRect = _container.rect;
			var radius = 0.5f * Mathf.Min( containerRect.width, containerRect.height );
			var center = new Vector2( containerRect.width * 0.5f, containerRect.height * 0.5f );

			foreach ( var avatar in _markers.Keys )
			{
				var avatarPos = avatar.position;
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

				if ( marker.CanRotate )
				{
					marker.transform.localRotation = (markerPos - center).ToLookRotation();//GetMarkerDirection( avatar );
				}
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
