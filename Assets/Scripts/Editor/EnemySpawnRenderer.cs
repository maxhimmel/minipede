using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	[System.Serializable]
    public class EnemySpawnRenderer
	{
		[ListDrawerSettings( 
			HideAddButton = true, HideRemoveButton = true, 
			ShowPaging = false, ShowItemCount = false,
			DraggableItems = false, Expanded = true )]
		[Space, ToggleGroup( "Main/_drawPlacements" )]
		[SerializeField] private List<DrawPlacementData> _placements = new List<DrawPlacementData>()
		{
			new DrawPlacementData( "_bee" ),
			new DrawPlacementData( "_beetle" ),
			new DrawPlacementData( "_dragonfly" ),
			new DrawPlacementData( "_earwig" ),
			new DrawPlacementData( "_minipede" ),
			new DrawPlacementData( "_mosquito" )
		};

		[TitleGroup( "Enemy Placements", "View the graph and modify where enemies are allowed to spawn.", GroupID = "Main", Order = -1 )]
		[ToggleGroup( "Main/_drawPlacements", ToggleGroupTitle = "Draw", Order = 0 )]
		[SerializeField] private bool _drawPlacements = true;

		private LevelGraphWrapper _levelGraphWrapper;
		private SerializedObject _enemySettingsObj;
		private Dictionary<SerializedProperty, DrawPlacementData> _placementData;

		public void AttachEnemySettings( SerializedObject settings )
		{
			_enemySettingsObj = settings;
		}

		public void OnSceneGui( SceneView obj )
		{
			if ( !_drawPlacements )
			{
				return;
			}

			CacheReferences();
			if ( !Application.isPlaying )
			{
				_levelGraphWrapper.Update();
			}

			foreach ( var kvp in _placementData )
			{
				if ( kvp.Value.Toggle )
				{
					DrawPlacement( kvp.Key, kvp.Value.Color );
				}
			}
		}

		private void CacheReferences()
		{
			if ( _levelGraphWrapper == null )
			{
				_levelGraphWrapper = new LevelGraphWrapper();
			}
			_levelGraphWrapper.RefreshReferences();

			_placementData = new Dictionary<SerializedProperty, DrawPlacementData>( _placements.Count );
			foreach ( var placement in _placements )
			{
				_placementData.Add(
					_enemySettingsObj.FindProperty( placement.Name ).FindPropertyRelative( "SpawnPlacement" ),
					placement
				);
			}
		}

		private void DrawPlacement( SerializedProperty placementProperty, Color color )
		{
			Handles.color = color;

			Vector2 levelGraphPos = _levelGraphWrapper.LevelGraph.transform.position;
			LevelGraph.Settings graphSettings = _levelGraphWrapper.GraphSettings;

			for ( int idx = 0; idx < placementProperty.arraySize; ++idx )
			{
				var spawnProperty = placementProperty.GetArrayElementAtIndex( idx );
				
				var areaProperty = spawnProperty.FindPropertyRelative( "Area" );
				GraphArea graphArea = new GraphArea()
				{
					RowCol = areaProperty.FindPropertyRelative( "RowCol" ).vector2IntValue,
					Size = areaProperty.FindPropertyRelative( "Size" ).vector2IntValue
				};

				Vector2 startPos = graphSettings.CellCoordToWorldPos( graphArea.RowCol );
				startPos -= graphSettings.Size / 2;

				RectInt area = new RectInt(
					Mathf.RoundToInt( startPos.x + levelGraphPos.x ), Mathf.RoundToInt( startPos.y + levelGraphPos.y ),
					Mathf.RoundToInt( graphArea.Size.x * graphSettings.Size.x ), Mathf.RoundToInt( graphArea.Size.y * graphSettings.Size.y )
				);

				// TODO: DRAW DOTTED LINES, PLZ
				Handles.DrawSolidRectangleWithOutline( area.ToRect(), color.MultAlpha( 0.3f ), color );

				DrawRotation( spawnProperty, area );
			}
		}

		private void DrawRotation( SerializedProperty spawnProperty, RectInt area )
		{
			var rotationProperty = spawnProperty.FindPropertyRelative( "Rotation" );
			var itemsProperty = rotationProperty.FindPropertyRelative( "_items" );
			for ( int node = 0; node < itemsProperty.arraySize; ++node )
			{
				var nodeProperty = itemsProperty.GetArrayElementAtIndex( node );
				var itemProperty = nodeProperty.FindPropertyRelative( "Item" );

				Quaternion arrowRot = Quaternion.LookRotation( Vector2.up.Rotate( itemProperty.intValue ) );
				Handles.ArrowHandleCap( 0, area.position.ToVector2(), arrowRot, 1f, EventType.Repaint );
			}
		}

		[System.Serializable]
		private class DrawPlacementData
		{
			[HorizontalGroup( MaxWidth = 0.2f, LabelWidth = 20 )]
			[ToggleLeft, LabelText( "$Name", NicifyText = true )]
			public bool Toggle = true;

			[HorizontalGroup, ShowIf( nameof( Toggle ) )]
			[HideLabel] public Color Color = Color.white;

			[HideInInspector] public string Name;

			public DrawPlacementData( string name )
			{
				Name = name;
			}
		}
	}
}
