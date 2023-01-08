using System;
using System.Collections.Generic;
using Minipede.Gameplay.Enemies;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Minipede.Editor
{
    [System.Serializable]
    public class EnemySpawnSimulator
    {
        [TitleGroup( "Enemies", "Simulates 'OnSpawn' being called for enemies directly placed in the scene." )]
        [HideLabel, EnumToggleButtons]
        [SerializeField] private EnemyType _enemies = EnemyType.All;

        [Flags]
        private enum EnemyType
        {
            All = -1,
            None = 0,

            Bee = 1 << 0,
            Beetle = 1 << 1,
            Dragonfly = 1 << 2,
            Earwig = 1 << 3,
            Inchworm = 1 << 4,
            Minipede = 1 << 5,
            Mosquito = 1 << 6,
            Segment = 1 << 7,
            Spider = 1 << 8
        }

        private Dictionary<Type, EnemyType> _typeLookup = new Dictionary<Type, EnemyType>()
        {
            { typeof( BeeController ), EnemyType.Bee },
            { typeof( BeetleController ), EnemyType.Beetle },
            { typeof( DragonflyController ), EnemyType.Dragonfly },
            { typeof( EarwigController ), EnemyType.Earwig },
            { typeof( InchwormController ), EnemyType.Inchworm },
            { typeof( MinipedeController ), EnemyType.Minipede },
            { typeof( MosquitoController ), EnemyType.Mosquito },
            { typeof( SegmentController ), EnemyType.Segment },
            { typeof( SpiderController ), EnemyType.Spider }
        };

        public void OnPlayModeChanged( PlayModeStateChange obj )
        {
            if ( obj != PlayModeStateChange.EnteredPlayMode )
            {
                return;
            }

            EnemyController[] allEnemies = GameObject.FindObjectsOfType<EnemyController>( includeInactive: false );
            foreach ( var enemy in allEnemies )
            {
                if ( !_typeLookup.TryGetValue( enemy.GetType(), out EnemyType typeId ) )
                {
                    throw new KeyNotFoundException( $"Please update the '<b>{nameof( _typeLookup )}<b>' and '<b>{nameof( EnemyType )}</b>'" );
                }

                if ( (typeId & _enemies) != 0 )
                {
                    enemy.StartMainBehavior();
                    Debug.Log( $"<color=orange>[{nameof( EnemySpawnSimulator )}]</color> " +
                        $"Simulated 'OnSpawn' for <b>{enemy.name}</b>.", enemy );
                }
            }
        }
    }
}