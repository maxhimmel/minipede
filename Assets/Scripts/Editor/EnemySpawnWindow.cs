using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Minipede.Installers;

namespace Minipede.Editor
{
	public class EnemySpawnWindow : OdinEditorWindow
	{
		[HideLabel, TabGroup( "Simulate" )]
		[SerializeField] private EnemySpawnSimulator _simulator = new EnemySpawnSimulator();

		[HideLabel, TabGroup( "Placement" )]
		[SerializeField] private EnemySpawnPlacement _placement = new EnemySpawnPlacement();

		[FoldoutGroup( "Enemy Settings" )]
		[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		[SerializeField] private EnemySettings _enemySettings;

		private SerializedObject _enemySettingsObj;

		[MenuItem( "Minipede/Enemy Spawning" )]
		private static void OpenWindow()
		{
			GetWindow<EnemySpawnWindow>( "Enemy Spawning" ).Show();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorApplication.playModeStateChanged += _simulator.OnPlayModeChanged;
			SceneView.duringSceneGui += _placement.OnSceneGui;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorApplication.playModeStateChanged -= _simulator.OnPlayModeChanged;
			SceneView.duringSceneGui -= _placement.OnSceneGui;
		}

		protected override void Initialize()
		{
			base.Initialize();

			if ( _enemySettings == null )
			{
				_enemySettings = EditorUtility.FindAsset<EnemySettings>( "EnemySettings" );
			}

			if ( _enemySettingsObj == null )
			{
				_enemySettingsObj = new SerializedObject( _enemySettings );
				_placement.AttachEnemySettings( _enemySettingsObj );
			}
		}

		protected override void OnGUI()
		{
			if ( !Application.isPlaying )
			{
				_enemySettingsObj.UpdateIfRequiredOrScript();
			}

			base.OnGUI();
		}

		//private void OnSceneGui( SceneView obj )
		//{
		//	if ( !_drawPlacements )
		//	{
		//		return;
		//	}

		//	CacheReferences();
		//	if ( !Application.isPlaying )
		//	{
		//		_enemySettingsObj.UpdateIfRequiredOrScript();
		//		_levelGraphWrapper.Update();
		//	}

		//	foreach ( var kvp in _placementData )
		//	{
		//		if ( kvp.Value.Toggle )
		//		{
		//			DrawPlacement( kvp.Key, kvp.Value.Color );
		//		}
		//	}
		//}

		//private void CacheReferences()
		//{
		//	if ( _levelGraphWrapper == null )
		//	{
		//		_levelGraphWrapper = new LevelGraphWrapper();
		//	}
		//	_levelGraphWrapper.RefreshReferences();

		//	if ( _enemySettings == null )
		//	{
		//		_enemySettings = EditorUtility.FindAsset<EnemySettings>( "EnemySettings" );
		//	}

		//	if ( _enemySettingsObj == null )
		//	{
		//		_enemySettingsObj = new SerializedObject( _enemySettings );

		//		_placementData = new Dictionary<SerializedProperty, DrawPlacementData>( _placements.Count );
		//		foreach ( var placement in _placements )
		//		{
		//			_placementData.Add(
		//				_enemySettingsObj.FindProperty( placement.Name ).FindPropertyRelative( "SpawnPlacement" ),
		//				placement
		//			);
		//		}
		//	}
		//}

		//private void DrawPlacement( SerializedProperty placementProperty, Color color )
		//{
		//	Vector2 levelGraphPos = _levelGraphWrapper.LevelGraph.transform.position;
		//	LevelGraph.Settings graphSettings = _levelGraphWrapper.GraphSettings;

		//	for ( int idx = 0; idx < placementProperty.arraySize; ++idx )
		//	{
		//		var graphAreaProperty = placementProperty.GetArrayElementAtIndex( idx );
		//		GraphArea graphArea = new GraphArea()
		//		{
		//			RowCol = graphAreaProperty.FindPropertyRelative( "RowCol" ).vector2IntValue,
		//			Size = graphAreaProperty.FindPropertyRelative( "Size" ).vector2IntValue
		//		};

		//		Vector2 startPos = graphSettings.CellCoordToWorldPos( graphArea.RowCol );
		//		startPos -= graphSettings.Size / 2;

		//		RectInt area = new RectInt(
		//			Mathf.RoundToInt( startPos.x + levelGraphPos.x ), Mathf.RoundToInt( startPos.y + levelGraphPos.y ),
		//			Mathf.RoundToInt( graphArea.Size.x * graphSettings.Size.x ), Mathf.RoundToInt( graphArea.Size.y * graphSettings.Size.y )
		//		);

		//		Handles.DrawSolidRectangleWithOutline( area.ToRect(), color.MultAlpha( 0.3f ), color );
		//	}
		//}

		//[System.Serializable]
		//private class DrawPlacementData
		//{
		//	[HorizontalGroup( MaxWidth = 0.2f, LabelWidth = 20 )]
		//	[ToggleLeft, LabelText( "$Name", NicifyText = true )]
		//	public bool Toggle = true;

		//	[HorizontalGroup, ShowIf( nameof( Toggle ) )]
		//	[HideLabel] public Color Color = Color.white;

		//	[HideInInspector] public string Name;

		//	public DrawPlacementData( string name )
		//	{
		//		Name = name;
		//	}
		//}
	}
}
