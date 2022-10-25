using UnityEngine;
using UnityEditor;
using Minipede.Gameplay.LevelPieces;
using Minipede.Installers;

namespace Minipede.Editor
{
    public class LevelGraphWrapper
	{
		public LevelGraph LevelGraph { get; private set; }
		public GameplaySettings GameplaySettings { get; private set; }

		public LevelGraph.Settings GraphSettings => new LevelGraph.Settings()
		{
			Dimensions = _dimensionsProperty.vector2IntValue,
			Size = _sizeProperty.vector2Value,
			Offset = _offsetProperty.vector2Value
		};
		public Vector2Int Dimensions => _dimensionsProperty.vector2IntValue;
		public Vector2 Size => _sizeProperty.vector2Value;
		public Vector2 Offset => _offsetProperty.vector2Value;

		public int PlayerRows => _playerRowsProperty.intValue;
		public int PlayerRowDepth => _playerRowDepthProperty.intValue;

		private SerializedObject _gameplaySettingsObj;
		private SerializedProperty _levelSettingsProperty;
		private SerializedProperty _builderSettingsProperty;
		private SerializedProperty _graphSettingsProperty;
		private SerializedProperty _playerRowsProperty;
		private SerializedProperty _playerRowDepthProperty;
		private SerializedProperty _dimensionsProperty;
		private SerializedProperty _sizeProperty;
		private SerializedProperty _offsetProperty;

		public LevelGraphWrapper()
		{
			RefreshReferences();
		}

		public void RefreshReferences()
		{
			if ( LevelGraph == null )
			{
				LevelGraph = GameObject.FindObjectOfType<LevelGraph>();
			}

			if ( GameplaySettings == null )
			{
				GameplaySettings = EditorUtility.FindAsset<GameplaySettings>( "GameplaySettings" );
			}

			if ( _gameplaySettingsObj == null )
			{
				_gameplaySettingsObj = new SerializedObject( GameplaySettings );
				_levelSettingsProperty = _gameplaySettingsObj.FindProperty( "_levelSettings" );
				_builderSettingsProperty = _levelSettingsProperty.FindPropertyRelative( "Builder" );
				_graphSettingsProperty = _levelSettingsProperty.FindPropertyRelative( "Graph" );

				_playerRowsProperty = _builderSettingsProperty.FindPropertyRelative( "PlayerRows" );
				_playerRowDepthProperty = _builderSettingsProperty.FindPropertyRelative( "PlayerRowDepth" );

				_dimensionsProperty = _graphSettingsProperty.FindPropertyRelative( "Dimensions" );
				_sizeProperty = _graphSettingsProperty.FindPropertyRelative( "Size" );
				_offsetProperty = _graphSettingsProperty.FindPropertyRelative( "Offset" );
			}
		}

		public bool Update()
		{
			if ( _gameplaySettingsObj != null )
			{
				return _gameplaySettingsObj.UpdateIfRequiredOrScript();
			}

			return false;
		}
	}
}
