using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
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
			DraggableItems = false, ShowFoldout = false, OnTitleBarGUI = "RefreshPlacements" )]
		[Space, ToggleGroup( "Main/_drawPlacements" )]
		[SerializeField] private List<DrawPlacementData> _placements = new List<DrawPlacementData>();

		[TitleGroup( "Enemy Placements", "View the graph and modify where enemies are allowed to spawn.", GroupID = "Main", Order = -1 )]
		[ToggleGroup( "Main/_drawPlacements", ToggleGroupTitle = "Draw Placements", Order = 0 )]
		[SerializeField] private bool _drawPlacements = true;

		private LevelGraphWrapper _levelGraphWrapper;
		private SerializedObject _enemySettingsObj;
		private Dictionary<SerializedProperty, DrawPlacementData> _placementData = new Dictionary<SerializedProperty, DrawPlacementData>();

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
					kvp.Key.serializedObject.UpdateIfRequiredOrScript();
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

			var enemyInstallersProperty = _enemySettingsObj.FindProperty( "_spawnSettings" );
			int enemyCount = enemyInstallersProperty.arraySize;

			if ( _placements.Count != enemyCount )
			{
				for ( int idx = 0; idx < enemyCount; ++idx )
				{
					var installerProperty = enemyInstallersProperty.GetArrayElementAtIndex( idx );
					var installerObj = new SerializedObject( installerProperty.objectReferenceValue );
					string enemyName = installerProperty.objectReferenceValue.name;

					var foundData = _placements.Find( data => data.Name == enemyName );
					if ( foundData != null )
					{
						continue;
					}

					var data = new DrawPlacementData( enemyName );
					_placements.Add( data );
				}
			}
			if ( _placementData.Count != _placements.Count )
			{
				_placementData = new Dictionary<SerializedProperty, DrawPlacementData>( enemyCount );
				for ( int idx = 0; idx < enemyCount; ++idx )
				{
					var installerProperty = enemyInstallersProperty.GetArrayElementAtIndex( idx );
					var installerObj = new SerializedObject( installerProperty.objectReferenceValue );
					string enemyName = installerProperty.objectReferenceValue.name;

					_placementData.Add( installerObj.FindProperty( "_spawnPlacement" ), _placements[idx] );
				}
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

		private void RefreshPlacements()
		{
			if ( SirenixEditorGUI.ToolbarButton( EditorIcons.Refresh ) )
			{
				var enemyInstallersProperty = _enemySettingsObj.FindProperty( "_spawnSettings" );
				int enemyCount = enemyInstallersProperty.arraySize;

				_placements = new List<DrawPlacementData>( enemyCount );
				for ( int idx = 0; idx < enemyCount; ++idx )
				{
					var installerProperty = enemyInstallersProperty.GetArrayElementAtIndex( idx );
					string enemyName = installerProperty.objectReferenceValue.name;

					var data = new DrawPlacementData( enemyName );
					_placements.Add( data );
				}

				_placementData = new Dictionary<SerializedProperty, DrawPlacementData>( enemyCount );
				for ( int idx = 0; idx < enemyCount; ++idx )
				{
					var installerProperty = enemyInstallersProperty.GetArrayElementAtIndex( idx );
					var installerObj = new SerializedObject( installerProperty.objectReferenceValue );

					_placementData.Add( installerObj.FindProperty( "_spawnPlacement" ), _placements[idx] );
				}
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
