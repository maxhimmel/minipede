using System;
using System.Collections.Generic;
using Minipede.Gameplay.Enemies;
using Minipede.Gameplay.UI;
using Minipede.Installers;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
	public class MarkerDangerReaction : IDangerWarningReaction,
		ILateTickable
	{
		public const string ContainerId = "Container";
		private readonly Settings _settings;
		private readonly Camera _camera;
		private readonly MonoMemoryPool<Transform> _markerPool;
		private readonly Transform _owner;
		private readonly Minimap _minimap;
		private readonly Transform _container;
		private readonly ISpaceConverter _spaceConverter;
		private readonly List<EnemyController> _enemies;
		private readonly Dictionary<EnemyController, Transform> _markers;

		public MarkerDangerReaction( Settings settings,
			Camera camera,
			//MonoMemoryPool<Transform> markerPool,
			Transform owner,
			Minimap minimap )
			//[Inject( Id = ContainerId )] Transform container,
			//ISpaceConverter spaceConverter )
		{
			_settings = settings;
			_camera = camera;
			//_markerPool = markerPool;
			_owner = owner;
			_minimap = minimap;
			//_container = container;
			//_spaceConverter = spaceConverter;

			_enemies = new List<EnemyController>();
			_markers = new Dictionary<EnemyController, Transform>();
		}

		public void React( EnemyController enemy )
		{
			_enemies.Add( enemy );

			_minimap.AddMarker( enemy.transform, _settings.MarkerPrefab.GetComponent<MinimapMarker>() );
			//var marker = _markerPool.Spawn();
			//_markers.Add( enemy, marker );
		}

		public void Neglect( EnemyController enemy )
		{
			_enemies.Remove( enemy );

			_minimap.RemoveMarker( enemy.transform );
			//if ( _markers.Remove( enemy, out var marker ) )
			//{
			//	_markerPool.Despawn( marker );
			//}
		}

		public void LateTick()
		{
			//var cameraBounds = _camera.GetWorldSpaceBounds();
			//var cameraRect = _camera.GetWorldSpaceRect();
			//var origin = _owner.position;

			//foreach ( var enemy in _enemies )
			//{
			//	var enemyPos = enemy.transform.position;
			//	var relativeDir = _container.InverseTransformDirection( enemyPos - origin );//enemyPos - origin;

			//	#region moved into interfaces
			//	//#region deprecated
			//	////var localPos = _container.InverseTransformDirection( relativeDir );
			//	////float dist = localPos.magnitude;
			//	////Debug.Log( $"w:{relativeDir.magnitude} | l:{dist}" );
			//	////if ( dist < _settings.Radius )
			//	////{
			//	////	localPos = localPos / dist * _settings.Radius;
			//	////}

			//	///// LOCAL POS AND RELATIVE DIR ARE THE SAME VECTORS - EXCEPT EXCEPT EXCEPT WHEN THE CONTAINER ROTATES (such as the explorer)
			//	////Debug.DrawRay( origin, localPos, Color.red );
			//	////Debug.DrawRay( origin, relativeDir, Color.green );
			//	//#endregion

			//	//float verticalDot = Vector2.Dot( relativeDir, Vector2.up );
			//	//float horizontalDot = Vector2.Dot( relativeDir, Vector2.right );

			//	//float verticalAspect = verticalDot / cameraRect.height * 2f;
			//	//float horizontalAspect = horizontalDot / cameraRect.height * 2f;

			//	//Vector2 normalizedDir = relativeDir / cameraRect.height * 2f;//new Vector2( horizontalAspect, verticalAspect );

			//	//// camera's width / height
			//	//float aspect = _camera.aspect;

			//	//#region debug log/draw
			//	////Debug.Log( 
			//	////	$"V | raw:{verticalDot} | aspect:{verticalAspect}\n" +
			//	////	$"H | raw:{ horizontalDot} | aspect:{horizontalAspect}\n" +
			//	////	$"aspect length:{normalizedDir.magnitude}"
			//	////);
			//	//Debug.DrawRay( origin, relativeDir, Color.red );
			//	//Debug.DrawRay( origin, normalizedDir, Color.green );
			//	//#endregion

			//	//if ( normalizedDir.sqrMagnitude > 1 )
			//	//{
			//	//	normalizedDir.Normalize();
			//	//}
			//	#endregion

			//	var convertedPos = _spaceConverter.Convert( enemyPos );//relativeDir );

			//	var marker = _markers[enemy];
			//	marker.transform.localPosition = convertedPos;// * _settings.Radius;
			//	marker.transform.localRotation = (enemyPos - origin).ToLookRotation();
			//}
		}

		public interface ISpaceConverter
		{
			Vector2 Convert( Vector2 position );
		}

		public class ScreenSpaceRadiusConverter : ISpaceConverter
		{
			private readonly float _radius;
			private readonly Camera _camera;
			private readonly Transform _owner;
			private readonly Transform _container;

			public ScreenSpaceRadiusConverter( float radius,
				Camera camera,
				Transform owner )
				//[Inject( Id = ContainerId )] Transform container )
			{
				_radius = radius;
				_camera = camera;
				_owner = owner;
				//_container = container;
			}

			public Vector2 Convert( Vector2 position )
			{
				var viewportPos = _camera.WorldToViewportPoint( position );
				var clampedViewportPos = new Vector2( Mathf.Clamp01( viewportPos.x ), Mathf.Clamp01( viewportPos.y ) );

				var rt = _container as RectTransform;
				if ( rt != null )
				{
					var containerRect = rt.rect;
					//var screenSize = new Vector2( Screen.width, Screen.height );

					//float maxHorizontal = containerRect.width / screenSize.x;
					//float maxVertical = containerRect.height / screenSize.y;

					//var clampedViewportPos = new Vector2()
					//{
					//	// zero cannot be here - we need to calculate a min hori/vert
					//	x = Mathf.Clamp( viewportPos.x, 0, maxHorizontal ), 
					//	y = Mathf.Clamp( viewportPos.y, 0, maxVertical )
					//};

					//Vector2 pos = new Vector2()
					//{
					//	x = clampedViewportPos.x * screenSize.x,
					//	y = clampedViewportPos.y * screenSize.y
					//};

					return new Vector2()
					{
						x = clampedViewportPos.x * containerRect.width,
						y = clampedViewportPos.y * containerRect.height
					};
				}

				return position;




				//var camSize = _camera.GetSize();

				//float horizontalDot = Vector2.Dot( position, Vector2.right );
				//float verticalDot = Vector2.Dot( position, Vector2.up );

				//float horizontalAspect = horizontalDot / camSize.x * 2f * _camera.aspect;
				//float verticalAspect = verticalDot / camSize.y * 2f;

				////Vector2 convertedPos = position / camSize.y * 2f;
				//Vector2 convertedPos = new Vector2( horizontalAspect, verticalAspect );

				//#region debug log/draw
				////Debug.Log( 
				////	$"V | raw:{verticalDot} | aspect:{verticalAspect}\n" +
				////	$"H | raw:{ horizontalDot} | aspect:{horizontalAspect}\n" +
				////	$"aspect length:{normalizedDir.magnitude}"
				////);
				////Debug.Log( $"o:{horizontalDot / camSize.x * 2f} | a:{horizontalDot * 2f / camSize.y}" );
				//Debug.Log( $"h:{2 * horizontalDot / camSize.y} | v:{2 * verticalDot / camSize.x}" );

				//Debug.DrawRay( _owner.position, position, Color.red );
				//Debug.DrawRay( _owner.position, convertedPos, Color.green );
				//#endregion

				//if ( convertedPos.sqrMagnitude > 1 )
				//{
				//	convertedPos.Normalize();
				//}

				//return convertedPos * _radius;
			}
		}

		public class WorldSpaceRadiusConverter : ISpaceConverter
		{
			private readonly float _radius;

			public WorldSpaceRadiusConverter( float radius )
			{
				_radius = radius;
			}

			public Vector2 Convert( Vector2 position )
			{
				float lengthSqr = position.sqrMagnitude;
				if ( lengthSqr > _radius * _radius )
				{
					position.Normalize();
				}

				return position * _radius;
			}
		}

		[System.Serializable]
		public struct Settings : IDangerWarningReaction.ISettings
		{
			public Type InstallerType => typeof( MarkerDangerReactionInstaller );

			public string PoolContainerId;
			public int InitialPoolSize;
			public GameObject MarkerPrefab;

			//[Space]
			//public float Radius; // Bounds, Camera;
		}
	}
}
