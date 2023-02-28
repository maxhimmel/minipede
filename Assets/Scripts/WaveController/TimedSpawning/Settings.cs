using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Waves
{
	public partial class TimedEnemySpawner
	{
		public interface ISettings : ITimedSpawner.ISettings
		{
			public WeightedListEnemy Enemies { get; }

			public float StartDelay { get; }
			public float SpawnFrequency { get; }

			public bool LimitEnemies { get; }
			public int MaxEnemies { get; }

			public int SwarmAmount { get; }
			public float SpawnStagger { get; }
			public bool UseNewEnemyPerSpawn { get; }
		}

		[System.Serializable]
		public class Settings : ISettings
		{
			public virtual System.Type SpawnerType => typeof( TimedEnemySpawner );
			public string Name => _name;

			public WeightedListEnemy Enemies => _enemies;

			public float StartDelay => _startDelay;
			public virtual float SpawnFrequency => _spawnFrequency.Random();

			public bool LimitEnemies => _limitEnemies;
			public virtual int MaxEnemies => _maxEnemies;

			public virtual int SwarmAmount => _swarmRange.Random();
			public float SpawnStagger => _spawnStagger;
			public bool UseNewEnemyPerSpawn => _useNewEnemyPerSpawn;

			[SerializeField] private string _name;

			[TabGroup( "Main", "Enemies" )]
			[SerializeField] private WeightedListEnemy _enemies;

			[TabGroup( "Main", "Settings" )]
			[MinValue( 0 )]
			[SerializeField] private float _startDelay;

			[TabGroup( "Main", "Settings" )]
			[MinMaxSlider( 0, 60, ShowFields = true )]
			[SerializeField] private Vector2 _spawnFrequency;

			[ToggleGroup( "_limitEnemies", "Limit Enemies", GroupID = "Main/Settings/LimitEnemies" )]
			[SerializeField] private bool _limitEnemies;

			[ToggleGroup( "_limitEnemies", GroupID = "Main/Settings/LimitEnemies" )]
			[MinValue( 1 )]
			[SerializeField] private int _maxEnemies;

			[PropertyTooltip( "How many enemies to spawn per cycle." )]
			[FoldoutGroup( "@GetSwarmingGroupName()", GroupID = "Main/Settings/Swarming" )]
			[MinMaxSlider( 1, 50, ShowFields = true )]
			[SerializeField] private Vector2Int _swarmRange = Vector2Int.one;

			[PropertyTooltip( "Delay between swarm spawns." )]
			[FoldoutGroup( "@GetSwarmingGroupName()", GroupID = "Main/Settings/Swarming" )]
			[ShowIf( "@IsSwarm()" )]
			[MinValue( 0 )]
			[SerializeField] private float _spawnStagger = 0;

			[PropertyTooltip( "Randomize enemy per swarm spawn?" )]
			[FoldoutGroup( "@GetSwarmingGroupName()", GroupID = "Main/Settings/Swarming" ), LabelText( "Swarm Variance" )]
			[ShowIf( "@IsSwarm() && HasVariance()" )]
			[SerializeField] private bool _useNewEnemyPerSpawn;

			#region Editor Tooling
#if UNITY_EDITOR
			private string GetSwarmingGroupName()
			{
				int min = _swarmRange.x;
				int max = _swarmRange.y;

				if ( min != max )
				{
					return $"Swarming ({min} - {max})";
				}
				else
				{
					return $"Swarming ({min})";
				}
			}

			private bool IsSwarm()
			{
				return _swarmRange.x > 1 || _swarmRange.y > 1;
			}

			private bool HasVariance()
			{
				return _enemies != null && _enemies.Count > 1;
			}
#endif
			#endregion
		}
	}
}
