using System;
using Minipede.Gameplay.Enemies.Spawning.Serialization;
using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelBalanceController
	{
		public Settings Balance { get; }
		public int Cycle { get; private set; }

		private readonly SignalBus _signalBus;

		public LevelBalanceController( Settings settings,
			SignalBus signalBus )
		{
			Balance = settings;
			Cycle = settings.StartCycle;

			_signalBus = signalBus;
		}

		public void IncrementCycle()
		{
			_signalBus.Fire( new LevelCycleChangedSignal( ++Cycle ) );
			Debug.Log( $"<color=orange>Level cycle incremented:</color> (<b>{Cycle}</b>)" );
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 0 )]
			public int StartCycle;

			[BoxGroup( "Enemy" ), LabelText( "Speed" )]
			public AnimationCurve EnemySpeed;
			[BoxGroup( "Enemy" ), LabelText( "Health" )]
			public AnimationCurve EnemyHealth;
			[BoxGroup( "Enemy" ), LabelText( "Frequency" )]
			public AnimationCurve EnemyFrequency;
			[BoxGroup( "Enemy" ), LabelText( "Amount" )]
			public AnimationCurve EnemyAmount;

			[BoxGroup( "Mushroom" ), LabelText( "Health" )]
			public AnimationCurve MushroomHealth;
		}
	}

	public class TimedWaveBalancer : TimedEnemySpawner.ISettings
	{
		public Type SpawnerType {
			get {
				return _settings.SpawnerType;
			}
		}

		public string Name {
			get {
				return _settings.Name;
			}
		}

		public WeightedListEnemy Enemies {
			get {
				return _settings.Enemies;
			}
		}

		public float StartDelay {
			get {
				return _settings.StartDelay;
			}
		}

		public float SpawnFrequency {
			get {
				var balance = _levelBalancer.Balance.EnemyFrequency.Evaluate( _levelBalancer.Cycle );
				return Mathf.Max( 1, _settings.SpawnFrequency + balance );
			}
		}

		public bool LimitEnemies {
			get {
				return _settings.LimitEnemies;
			}
		}

		public int MaxEnemies {
			get {
				return _settings.MaxEnemies;
			}
		}

		public int SwarmAmount {
			get {
				var balance = Mathf.FloorToInt( _levelBalancer.Balance.EnemyAmount.Evaluate( _levelBalancer.Cycle ) );
				return _settings.SwarmAmount + balance;
			}
		}

		public float SpawnStagger {
			get {
				return _settings.SpawnStagger;
			}
		}

		public bool UseNewEnemyPerSpawn {
			get {
				return _settings.UseNewEnemyPerSpawn;
			}
		}

		private readonly TimedEnemySpawner.ISettings _settings;
		private readonly LevelBalanceController _levelBalancer;

		public TimedWaveBalancer( TimedEnemySpawner.ISettings settings,
			LevelBalanceController levelBalancer )
		{
			_settings = settings;
			_levelBalancer = levelBalancer;
		}

		public TSettings Cast<TSettings>() 
			where TSettings : TimedEnemySpawner.Settings
		{
			return _settings as TSettings;
		}
	}
}