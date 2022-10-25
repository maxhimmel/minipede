using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using Minipede.Installers;
using Minipede.Utility;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
	[System.Serializable]
    public class EnemySpawnPlacement
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

		[BoxGroup( "Main", ShowLabel = false, Order = 0 )]
		[ToggleGroup( "_drawPlacements", ToggleGroupTitle = "Draw", GroupID = "Main/_drawPlacements" )]
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
				//_enemySettingsObj.UpdateIfRequiredOrScript();
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

			//if ( _enemySettings == null )
			//{
			//	_enemySettings = EditorUtility.FindAsset<EnemySettings>( "EnemySettings" );
			//}

			//if ( _enemySettingsObj == null )
			//{
			//	_enemySettingsObj = new SerializedObject( _enemySettings );

			_placementData = new Dictionary<SerializedProperty, DrawPlacementData>( _placements.Count );
			foreach ( var placement in _placements )
			{
				_placementData.Add(
					_enemySettingsObj.FindProperty( placement.Name ).FindPropertyRelative( "SpawnPlacement" ),
					placement
				);
			}
			//}
		}

		private void DrawPlacement( SerializedProperty placementProperty, Color color )
		{
			Vector2 levelGraphPos = _levelGraphWrapper.LevelGraph.transform.position;
			LevelGraph.Settings graphSettings = _levelGraphWrapper.GraphSettings;

			for ( int idx = 0; idx < placementProperty.arraySize; ++idx )
			{
				var graphAreaProperty = placementProperty.GetArrayElementAtIndex( idx );
				GraphArea graphArea = new GraphArea()
				{
					RowCol = graphAreaProperty.FindPropertyRelative( "RowCol" ).vector2IntValue,
					Size = graphAreaProperty.FindPropertyRelative( "Size" ).vector2IntValue
				};

				Vector2 startPos = graphSettings.CellCoordToWorldPos( graphArea.RowCol );
				startPos -= graphSettings.Size / 2;

				RectInt area = new RectInt(
					Mathf.RoundToInt( startPos.x + levelGraphPos.x ), Mathf.RoundToInt( startPos.y + levelGraphPos.y ),
					Mathf.RoundToInt( graphArea.Size.x * graphSettings.Size.x ), Mathf.RoundToInt( graphArea.Size.y * graphSettings.Size.y )
				);

				Handles.DrawSolidRectangleWithOutline( area.ToRect(), color.MultAlpha( 0.3f ), color );
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
		//private readonly string[] _enemyPropertyNames = new string[] {
		//	"_bee", "_beetle", "_dragonfly", "_earwig", "_minipede", "_mosquito"
		//};

		//[BoxGroup( "Main", ShowLabel = false )]
		//[ToggleGroup( "_drawPlacements", ToggleGroupTitle = "Draw Placements", GroupID = "Main/_drawPlacements" )]
		//[SerializeField] private bool _drawPlacements = true;

		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _bee = new DrawPlacementData( "Bee" );
		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _beetle = new DrawPlacementData( "Beetle" );
		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _dragonfly = new DrawPlacementData( "Dragonfly" );
		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _earwig = new DrawPlacementData( "Earwig" );
		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _minipede = new DrawPlacementData( "Minipede" );
		//[HideLabel, ToggleGroup( "Main/_drawPlacements" )]
		//[SerializeField] private DrawPlacementData _mosquito = new DrawPlacementData( "Mosquito" );

		//[FoldoutGroup( "Enemy Settings" )]
		//[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		//[SerializeField] private EnemySettings _enemySettings;

		//private LevelGraphWrapper _levelGraphWrapper;
		//private SerializedObject _placementWindowObj;
		//private SerializedObject _enemySettingsObj;
		//private Dictionary<SerializedProperty, SerializedProperty> _placementProperties;

		//[MenuItem( "Minipede/Enemy Spawn Placement" )]
		//private static void OpenWindow()
		//{
		//	GetWindow<EnemySpawnPlacementWindow>( "Enemy Placement" ).Show();
		//}

		//protected override void OnEnable()
		//{
		//	base.OnEnable();

		//	SceneView.duringSceneGui += OnSceneGui;
		//}

		//protected override void OnDestroy()
		//{
		//	base.OnDestroy();

		//	SceneView.duringSceneGui -= OnSceneGui;
		//}

		//public void DrawGui()
		//{
		//	base.OnGUI();
		//}

		//private void OnSceneGui( SceneView obj )
		//{
		//	if ( !_drawPlacements )
		//	{
		//		return;
		//	}

		//	CacheReferences();
		//	if ( !Application.isPlaying )
		//	{
		//		_placementWindowObj.UpdateIfRequiredOrScript();
		//		_enemySettingsObj.UpdateIfRequiredOrScript();
		//		_levelGraphWrapper.Update();
		//	}

		//	foreach ( var kvp in _placementProperties )
		//	{
		//		bool isToggled = kvp.Value.FindPropertyRelative( "Toggle" ).boolValue;
		//		if ( isToggled )
		//		{
		//			Color color = kvp.Value.FindPropertyRelative( "Color" ).colorValue;
		//			DrawPlacement( kvp.Key, color );
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
		//		_placementWindowObj = new SerializedObject( this );

		//		_placementProperties = new Dictionary<SerializedProperty, SerializedProperty>( _enemyPropertyNames.Length );
		//		foreach ( string enemyName in _enemyPropertyNames )
		//		{
		//			_placementProperties.Add(
		//				/*placement: */	_enemySettingsObj.FindProperty( enemyName ).FindPropertyRelative( "SpawnPlacement" ),
		//				/*DrawPlacementData: */ _placementWindowObj.FindProperty( enemyName )
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
		//	[ToggleLeft, LabelText( "$_name" )]
		//	public bool Toggle = true;

		//	[HorizontalGroup, ShowIf( nameof( Toggle ) )]
		//	[HideLabel] public Color Color = Color.white;

		//	[SerializeField, HideInInspector] private string _name;

		//	public DrawPlacementData( string name )
		//	{
		//		_name = name;
		//	}
		//}
	}
}
