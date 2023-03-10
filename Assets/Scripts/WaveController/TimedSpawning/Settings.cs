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
			WeightedListEnemy Enemies { get; }

			float StartDelay { get; }
			float SpawnFrequency { get; }

			bool LimitEnemies { get; }
			int MaxEnemies { get; }

			int SwarmAmount { get; }
			float SpawnStagger { get; }
			bool UseNewEnemyPerSpawn { get; }

			TSettings Cast<TSettings>()
				where TSettings : Settings;
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
			[MinValue( 0 )]
			[SerializeField] private float _spawnStagger = 0;

			[PropertyTooltip( "Randomize enemy per swarm spawn?" )]
			[FoldoutGroup( "@GetSwarmingGroupName()", GroupID = "Main/Settings/Swarming" ), LabelText( "Swarm Variance" )]
			[ShowIf( "@IsSwarm() && HasVariance()" )]
			[SerializeField] private bool _useNewEnemyPerSpawn;

			[Space, TabGroup( "Main", "Settings" ), InlineEditor]
			[ValidateInput( "IsBalanceTableValid", "Balance tables must match wave types." )]
			public EnemyWaveBalances Balances;

			public TSettings Cast<TSettings>()
				where TSettings : Settings
			{
				return this as TSettings;
			}

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

			private bool IsBalanceTableValid( EnemyWaveBalances balances, ref string errorMessage )
			{
				if ( balances == null )
				{
					return true;
				}

				if ( GetType() == typeof( TimedMinipedeSpawner.Settings ) )
				{
					if ( balances.GetType() != typeof( MinipedeWaveBalances ) )
					{
						errorMessage = "Please attach a 'Minipede Wave Balance Table'\n" +
							"Create via '<b>Minipede/Misc/...</b>' as a scriptable object.";
						return false;
					}
				}
				else
				{
					if ( balances.GetType() != typeof( EnemyWaveBalances ) )
					{
						errorMessage = "Please attach a 'Wave Balance Table'\n" +
							"Create via '<b>Minipede/Misc/...</b>' as a scriptable object.";
						return false;
					}
				}

				return true;
			}
#endif
			#endregion
		}
	}
}
