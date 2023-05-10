using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Minipede.Installers;
using Sirenix.Utilities.Editor;

namespace Minipede.Editor
{
	public class EnemySpawnWindow : OdinEditorWindow
	{
		[HideLabel, TabGroup( "Simulate" ), ShowIf( "CanInteract" )]
		[SerializeField] private EnemySpawnSimulator _simulator = new EnemySpawnSimulator();

		[HideLabel, TabGroup( "Placement" ), ShowIf( "CanInteract" )]
		[SerializeField] private EnemySpawnRenderer _renderer = new EnemySpawnRenderer();

		[FoldoutGroup( "Enemy Settings" ), ShowIf( "CanInteract" )]
		[InlineEditor( ObjectFieldMode = InlineEditorObjectFieldModes.CompletelyHidden )]
		[SerializeField] private EnemySettings _enemySettings;

		private readonly string _saveLoadKey = nameof( EnemySpawnWindow );
		private SerializedObject _enemySettingsObj;

		[MenuItem( "Minipede/Enemy Spawning" )]
		private static void OpenWindow()
		{
			GetWindow<EnemySpawnWindow>( "Enemy Spawning" ).Show();
		}

		private void Awake()
		{
			EditorUtility.LoadFromEditorPref( _saveLoadKey, this );
		}

		protected override void OnEnable()
		{
			base.OnEnable();

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

		private bool TryCacheEnemySettings()
		{
			if ( _enemySettings == null )
			{
				_enemySettings = GameObject.FindObjectOfType<EnemySettings>();
				if ( _enemySettings == null )
				{
					return false;
				}
			}

			if ( _enemySettingsObj == null || _enemySettingsObj.targetObject == null )
			{
				_enemySettingsObj = new SerializedObject( _enemySettings );
				_renderer.AttachEnemySettings( _enemySettingsObj );
			}

			return CanInteract();
		}

		protected override void OnGUI()
		{
			base.OnGUI();

			if ( !TryCacheEnemySettings() )
			{
				return;
			}

			if ( !Application.isPlaying )
			{
				_enemySettingsObj.UpdateIfRequiredOrScript();
			}
		}

		[OnInspectorGUI, HideIf( "CanInteract" )]
		private void DrawErrorMessage()
		{
			SirenixEditorGUI.ErrorMessageBox( $"This window requires a scene with an <b>{nameof( EnemySettings )}</b> component." );
		}

		private bool CanInteract()
		{
			return _enemySettingsObj != null;
		}
	}
}
