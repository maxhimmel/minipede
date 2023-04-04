﻿using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class WorldspaceMinimap : Minimap
	{
		[SerializeField] private bool _isRadial;

		private ExplorerController _explorer;

		[Inject]
		public void Construct( ExplorerController explorer )
		{
			_explorer = explorer;
		}

		protected override bool CanUpdate()
		{
			return base.CanUpdate()
				&& _explorer.Pawn != null;
		}

		protected override void UpdateMap()
		{
			FollowExplorer();

			var containerExtents = 0.5f * new Vector2( _container.rect.width, _container.rect.height );
			var radius = Mathf.Min( containerExtents.x, containerExtents.y );
			var explorerPos = _explorer.Pawn.Body.position;
			var camSize = _camera.GetWorldSpaceSize();

			foreach ( var avatar in _markers.Keys )
			{
				var avatarPos = avatar.transform.position.ToVector2();
				var markerDir = avatarPos - explorerPos;
				var markerPos = new Vector2()
				{
					x = markerDir.x / camSize.x * containerExtents.x,
					y = markerDir.y / camSize.y * containerExtents.y
				};

				if ( _isRadial )
				{
					if ( markerPos.sqrMagnitude > radius * radius )
					{
						markerPos = Vector3.ClampMagnitude( markerPos, radius );
					}
				}

				var marker = _markers[avatar];
				((RectTransform)(marker.transform)).anchoredPosition = markerPos;
				marker.transform.rotation = markerPos.ToLookRotation();
			}
		}

		private void FollowExplorer()
		{
			var explorerPos = _explorer.Pawn.Body.position;
			var viewportPos = _camera.WorldToViewportPoint( explorerPos );

			transform.position = new Vector2( viewportPos.x * Screen.width, viewportPos.y * Screen.height );
		}
	}
}