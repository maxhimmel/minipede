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
		[SerializeField] private EnemySpawnRenderer _renderer = new EnemySpawnRenderer();

		[FoldoutGroup( "Enemy Settings" )]
		[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		[SerializeField] private EnemySettings _enemySettings;

		private readonly string _saveLoadKey = nameof( EnemySpawnWindow );
		private SerializedObject _enemySettingsObj;

		[MenuItem( "Minipede/Enemy Spawning" )]
		private static void OpenWindow()
		{
			GetWindow<EnemySpawnWindow>( "Enemy Spawning" ).Show();
		}

		protected override void OnEnable()
		{
			base.OnEnable();

			EditorUtility.LoadFromEditorPref( _saveLoadKey, this );

			EditorApplication.playModeStateChanged += _simulator.OnPlayModeChanged;
			SceneView.duringSceneGui += _renderer.OnSceneGui;
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorUtility.SaveToEditorPref( _saveLoadKey, this );

			EditorApplication.playModeStateChanged -= _simulator.OnPlayModeChanged;
			SceneView.duringSceneGui -= _renderer.OnSceneGui;
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
				_renderer.AttachEnemySettings( _enemySettingsObj );
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
	}
}
