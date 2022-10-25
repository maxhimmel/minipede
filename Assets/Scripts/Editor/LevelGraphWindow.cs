using UnityEngine;
using UnityEditor;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Minipede.Installers;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;

namespace Minipede.Editor
{
	public class LevelGraphWindow : OdinEditorWindow
	{
		[BoxGroup( "Main", ShowLabel = false )] 
		[ToggleGroup( "_drawGraph", ToggleGroupTitle = "Draw Graph", GroupID = "Main/_drawGraph" )]
		[SerializeField] private bool _drawGraph = true;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" )]
		[SerializeField] private Color _gridColor = Color.red;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" )]
		[SerializeField] private Color _playerColor = Color.white;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" )]
		[SerializeField] private Color _playerDepthColor = Color.yellow;

		[FoldoutGroup( "Gameplay Settings" )] 
		[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		[SerializeField] private GameplaySettings _gameplaySettings;

		private readonly string _saveLoadKey = nameof( LevelGraphWindow );
		private LevelGraphWrapper _levelGraphWrapper;

		[MenuItem( "Minipede/Level Graph" )]
		private static void OpenWindow()
		{
			GetWindow<LevelGraphWindow>( "Level Graph" ).Show();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorUtility.LoadFromEditorPref( _saveLoadKey, this );

			SceneView.duringSceneGui += OnSceneGui;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorUtility.SaveToEditorPref( _saveLoadKey, this );

			SceneView.duringSceneGui -= OnSceneGui;
		}

		private void OnSceneGui( SceneView obj )
		{
			if ( !_drawGraph )
			{
				return;
			}

			CacheReferences();
			if ( !Application.isPlaying )
			{
				_levelGraphWrapper.Update();
			}

			// Player area ...
			Handles.color = _playerColor;
			DrawRowsAndColumns( 0, _levelGraphWrapper.PlayerRows - _levelGraphWrapper.PlayerRowDepth );

			// Level area ...
			Handles.color = _gridColor;
			DrawRowsAndColumns( _levelGraphWrapper.PlayerRows, _levelGraphWrapper.Dimensions.Row() );

			// Player depth area ...
			Handles.color = _playerDepthColor;
			DrawRowsAndColumns( _levelGraphWrapper.PlayerRows - _levelGraphWrapper.PlayerRowDepth, _levelGraphWrapper.PlayerRows );
		}

		private void CacheReferences()
		{
			if ( _levelGraphWrapper == null )
			{
				_levelGraphWrapper = new LevelGraphWrapper();
			}
			_levelGraphWrapper.RefreshReferences();

			_gameplaySettings = _levelGraphWrapper.GameplaySettings;
		}

		private void DrawRowsAndColumns( int startRow, int rowCount )
		{
			Vector2 pivot = _levelGraphWrapper.LevelGraph.transform.position;
			LevelGraph.Settings graphSettings = _levelGraphWrapper.GraphSettings;

			for ( int row = startRow; row < rowCount; ++row )
			{
				for ( int col = 0; col < graphSettings.Dimensions.Col(); ++col )
				{
					Vector2 pos = pivot + graphSettings.CellCoordToWorldPos( VectorExtensions.CreateRowCol( row, col ) );
					Handles.DrawWireCube( pos, graphSettings.Size );
				}
			}
		}
	}
}

#region Native Unity Editor (no gameplay settings inspector nesting)
//using UnityEngine;
//using UnityEditor;
//using Minipede.Gameplay.LevelPieces;
//using Minipede.Utility;
//using Minipede.Installers;

//namespace Minipede.Editor
//{
//	public class LevelGraphWindow : EditorWindow
//	{
//		private LevelGraph _levelGraph;

//		private SerializedObject _gameplaySettingsObj;
//		private SerializedProperty _levelSettingsProperty;
//		private SerializedProperty _builderSettingsProperty;
//		private SerializedProperty _graphSettingsProperty;
//		private SerializedProperty _playerRowsProperty;
//		private SerializedProperty _playerRowDepthProperty;
//		private SerializedProperty _dimensionsProperty;
//		private SerializedProperty _sizeProperty;
//		private SerializedProperty _offsetProperty;

//		private bool _drawGraph = true;
//		private Color _gridColor = Color.red;
//		private Color _playerColor = Color.white;
//		private Color _playerDepthColor = Color.yellow;

//		[MenuItem( "Minipede/Level Graph" )]
//		private static void OpenWindow()
//		{
//			GetWindow<LevelGraphWindow>( "Level Graph" ).Show();
//		}

//		private void OnEnable()
//		{
//			CacheReferences();

//			SceneView.duringSceneGui += OnSceneGui;
//		}

//		private void CacheReferences()
//		{
//			_levelGraph = FindObjectOfType<LevelGraph>();

//			string[] guids = AssetDatabase.FindAssets( "GameplaySettings" );
//			string path = AssetDatabase.GUIDToAssetPath( guids[0] );
//			var gameplaySettings = AssetDatabase.LoadAssetAtPath<GameplaySettings>( path );

//			_gameplaySettingsObj = new SerializedObject( gameplaySettings );
//			_levelSettingsProperty = _gameplaySettingsObj.FindProperty( "_levelSettings" );
//			_builderSettingsProperty = _levelSettingsProperty.FindPropertyRelative( "Builder" );
//			_graphSettingsProperty = _levelSettingsProperty.FindPropertyRelative( "Graph" );

//			_playerRowsProperty = _builderSettingsProperty.FindPropertyRelative( "PlayerRows" );
//			_playerRowDepthProperty = _builderSettingsProperty.FindPropertyRelative( "PlayerRowDepth" );

//			_dimensionsProperty = _graphSettingsProperty.FindPropertyRelative( "Dimensions" );
//			_sizeProperty = _graphSettingsProperty.FindPropertyRelative( "Size" );
//			_offsetProperty = _graphSettingsProperty.FindPropertyRelative( "Offset" );
//		}

//		private void OnDestroy()
//		{
//			SceneView.duringSceneGui -= OnSceneGui;
//		}

//		private void OnGUI()
//		{
//			using ( new EditorGUILayout.VerticalScope( EditorStyles.helpBox ) )
//			{
//				using ( var check = new EditorGUI.ChangeCheckScope() )
//				{
//					_drawGraph = EditorGUILayout.ToggleLeft( "DrawGraph", _drawGraph );

//					EditorGUILayout.Space();

//					_gridColor = EditorGUILayout.ColorField( "GridColor", _gridColor );
//					_playerColor = EditorGUILayout.ColorField( nameof( LevelBuilder.Settings.PlayerRows ), _playerColor );
//					_playerDepthColor = EditorGUILayout.ColorField( nameof( LevelBuilder.Settings.PlayerRowDepth ), _playerDepthColor );

//					if ( check.changed )
//					{
//						SceneView.RepaintAll();
//					}
//				}
//			}

//			using ( new EditorGUILayout.VerticalScope( EditorStyles.helpBox ) )
//			{
//				_gameplaySettingsObj.Update();
//				EditorGUILayout.PropertyField( _builderSettingsProperty );
//				EditorGUILayout.PropertyField( _graphSettingsProperty );
//				_gameplaySettingsObj.ApplyModifiedProperties();
//			}
//		}

//		private void OnSceneGui( SceneView obj )
//		{
//			if ( !_drawGraph )
//			{
//				return;
//			}

//			// Player area ...
//			Handles.color = _playerColor;
//			DrawRowsAndColumns( 0, _playerRowsProperty.intValue - _playerRowDepthProperty.intValue );

//			// Level area ...
//			Handles.color = _gridColor;
//			DrawRowsAndColumns( _playerRowsProperty.intValue, _dimensionsProperty.vector2IntValue.Row() );

//			// Player depth area ...
//			Handles.color = _playerDepthColor;
//			DrawRowsAndColumns( _playerRowDepthProperty.intValue, _playerRowsProperty.intValue );
//		}

//		private void DrawRowsAndColumns( int startRow, int rowCount )
//		{
//			Vector2 center = _levelGraph.transform.position;
//			Vector2 size = _sizeProperty.vector2Value;

//			Vector2 centerOffset = size * 0.5f;
//			centerOffset += _offsetProperty.vector2Value;

//			for ( int row = startRow; row < rowCount; ++row )
//			{
//				for ( int col = 0; col < _dimensionsProperty.vector2IntValue.Col(); ++col )
//				{
//					Vector2 pos = center + centerOffset
//						+ Vector2.up * row * size.y
//						+ Vector2.right * col * size.x;

//					Handles.DrawWireCube( pos, size );
//				}
//			}
//		}
//	}
//}
#endregion