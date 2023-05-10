using UnityEngine;
using UnityEditor;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Minipede.Installers;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;

namespace Minipede.Editor
{
	public class LevelGraphWindow : OdinEditorWindow
	{
		[BoxGroup( "Main", ShowLabel = false ), ShowIf( "CanInteract" )] 
		[ToggleGroup( "_drawGraph", ToggleGroupTitle = "Draw Graph", GroupID = "Main/_drawGraph" ), ShowIf( "CanInteract" )]
		[SerializeField] private bool _drawGraph = true;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" ), ShowIf( "CanInteract" )]
		[SerializeField] private Color _gridColor = Color.red;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" ), ShowIf( "CanInteract" )]
		[SerializeField] private Color _playerColor = Color.white;
		[ToggleGroup( "_drawGraph", GroupID = "Main/_drawGraph" ), ShowIf( "CanInteract" )]
		[SerializeField] private Color _playerDepthColor = Color.yellow;

		[BoxGroup( GroupID = "Main/_drawGraph/debug" ), ShowIf( "CanInteract" )]
		[HorizontalGroup( GroupID = "Main/_drawGraph/debug/CellCoord" ), ShowIf( "CanInteract" )]
		[SerializeField, ToggleLeft] private bool _debugCellCoord;
		[HorizontalGroup( GroupID = "Main/_drawGraph/debug/CellCoord" ), ShowIf( "@_debugCellCoord && CanInteract()" )]
		[SerializeField, HideLabel] private Vector2Int _cellCoord;
		[BoxGroup( GroupID = "Main/_drawGraph/debug" ), ShowIf( "@_debugCellCoord && CanInteract()" )]
		[SerializeField] private Color _cellCoordColor = Color.white;

		[FoldoutGroup( "Level Settings" ), ShowIf( "CanInteract" )] 
		[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		[SerializeField] private LevelGenerationInstaller _levelSettings;

		private readonly string _saveLoadKey = nameof( LevelGraphWindow );
		private LevelGraphWrapper _levelGraphWrapper;

		[MenuItem( "Minipede/Level Graph" )]
		private static void OpenWindow()
		{
			GetWindow<LevelGraphWindow>( "Level Graph" ).Show();
		}

		private void Awake()
		{
			EditorUtility.LoadFromEditorPref( _saveLoadKey, this );
		}

		protected override void OnEnable()
		{
			base.OnEnable();

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
			if ( !TryCacheReferences() )
			{
				return;
			}

			if ( !_drawGraph )
			{
				return;
			}

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

			DrawDebugCellCoord();
		}

		private bool TryCacheReferences()
		{
			if ( _levelGraphWrapper == null )
			{
				_levelGraphWrapper = new LevelGraphWrapper();
			}

			if ( _levelGraphWrapper.TryCacheReferences() )
			{
				_levelSettings = _levelGraphWrapper.LevelSettings;
			}

			return CanInteract();
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

		private void DrawDebugCellCoord()
		{
			if ( !_debugCellCoord )
			{
				return;
			}

			Vector2 pivot = _levelGraphWrapper.LevelGraph.transform.position;
			LevelGraph.Settings graphSettings = _levelGraphWrapper.GraphSettings;

			Vector2 pos = pivot + graphSettings.CellCoordToWorldPos( _cellCoord );
			Rect rect = new Rect( pos - graphSettings.Size / 2f, graphSettings.Size );

			Handles.color = _cellCoordColor;
			Handles.DrawSolidRectangleWithOutline( rect, _cellCoordColor.MultAlpha( 0.2f ), _cellCoordColor );
		}

		private bool CanInteract()
		{
			return _levelSettings != null;
		}

		[OnInspectorGUI, HideIf( "CanInteract" )]
		private void DrawErrorMessage()
		{
			SirenixEditorGUI.ErrorMessageBox( $"This window requires a scene with a <b>{nameof( LevelGraph )}</b> " +
				$"and a <b>{nameof( LevelGenerationInstaller )}</b> component." );
		}
	}
}