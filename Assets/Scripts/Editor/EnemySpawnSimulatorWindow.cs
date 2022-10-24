using System;
using System.Collections.Generic;
using Minipede.Gameplay.Enemies;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace Minipede.Editor
{
    public class EnemySpawnSimulatorWindow : OdinEditorWindow
    {
        [TitleGroup( "Enemies", "Simulates 'OnSpawn' being called for enemies directly placed in the scene." )]
        [HideLabel, EnumToggleButtons]
        [SerializeField] private EnemyType _enemies;

        [Flags]
        private enum EnemyType
        {
            All = -1,
            None = 0,

            Bee = 1 << 0,
            Beetle = 1 << 1,
            Dragonfly = 1 << 2,
            Earwig = 1 << 3,
            Minipede = 1 << 4,
            Mosquito = 1 << 5,
            Segment = 1 << 6
        }

        private Dictionary<Type, EnemyType> _typeLookup = new Dictionary<Type, EnemyType>()
        {
            { typeof( BeeController ), EnemyType.Bee },
            { typeof( BeetleController ), EnemyType.Beetle },
            { typeof( DragonflyController ), EnemyType.Dragonfly },
            { typeof( EarwigController ), EnemyType.Earwig },
            { typeof( MinipedeController ), EnemyType.Minipede },
            { typeof( MosquitoController ), EnemyType.Mosquito },
            { typeof( SegmentController ), EnemyType.Segment }
        };

        [MenuItem( "Minipede/Enemy Spawn Simulator" )]
        private static void OpenWindow()
        {
            GetWindow<EnemySpawnSimulatorWindow>( "Spawn Simulator" ).Show();
        }

		protected override void OnEnable()
        {
            base.OnEnable();

            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

		protected override void OnDestroy()
		{
			base.OnDestroy();

			EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        }

		private void OnPlayModeChanged( PlayModeStateChange obj )
		{
            if ( obj != PlayModeStateChange.EnteredPlayMode )
			{
                return;
			}

            EnemyController[] allEnemies = FindObjectsOfType<EnemyController>( includeInactive: false );
            foreach ( var enemy in allEnemies )
			{
                if ( !_typeLookup.TryGetValue( enemy.GetType(), out EnemyType typeId ) )
				{
                    throw new KeyNotFoundException( $"Please update the '<b>{nameof( _typeLookup )}<b>' and '<b>{nameof( EnemyType )}</b>'" );
				}

                if ( (typeId & _enemies) != 0 )
                {
                    enemy.OnSpawned();
                    Debug.Log( $"<color=orange>[{nameof(EnemySpawnSimulatorWindow)}]</color> " +
                        $"Simulated 'OnSpawn' for <b>{enemy.name}</b>.", enemy );
				}
			}
		}
    }
}
