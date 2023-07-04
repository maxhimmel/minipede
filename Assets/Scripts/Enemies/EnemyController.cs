using System;
using System.Threading;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Treasures;
using Minipede.Installers;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class EnemyController : MonoBehaviour,
		IDamageController,
		IPoolable<IOrientation, IMemoryPool>,
		IDisposable
	{
		public event IDamageController.OnHit Damaged {
			add => _damageController.Damaged += value;
			remove => _damageController.Damaged -= value;
		}
		public event IDamageController.OnHit Died {
			add => _damageController.Died += value;
			remove => _damageController.Died -= value;
		}

		public HealthController Health => _damageController.Health;
		public bool IsReady => _levelInitializer.IsReady;
		public bool IsAlive => _onDestroyCancelSource != null && !OnDestroyCancelToken.IsCancellationRequested;
		public CancellationToken OnDestroyCancelToken => _onDestroyCancelSource.Token;
		public Rigidbody2D Body => _body;

		private SharedSettings _sharedSettings;
		protected Rigidbody2D _body;
		private IDamageController _damageController;
		protected ILevelInitializer _levelInitializer;
		protected LevelGraph _levelGraph;
		protected LevelForeman _levelForeman;
		protected SignalBus _signalBus;
		private LootBox _lootBox;
		private LevelGenerationInstaller.Level _levelSettings;
		private IHealthBalanceResolver _healthBalancer;
		private ISpeedBalanceResolver _speedBalancer;

		private CancellationTokenSource _onDestroyCancelSource;
		private IMemoryPool _memoryPool;

		[Inject]
		public void Construct( SharedSettings sharedSettings,
			Rigidbody2D body,
			IDamageController damageController,
			ILevelInitializer levelInitializer, 
			LevelGraph levelGraph,
			LevelForeman foreman,
			SignalBus signalBus,
			LootBox lootBox,
			LevelGenerationInstaller.Level levelSettings,
			IHealthBalanceResolver healthBalancer,
			ISpeedBalanceResolver speedBalancer )
		{
			_sharedSettings = sharedSettings;
			_body = body;
			_damageController = damageController;
			_levelInitializer = levelInitializer;
			_levelGraph = levelGraph;
			_levelForeman = foreman;
			_signalBus = signalBus;
			_lootBox = lootBox;
			_levelSettings = levelSettings;
			_healthBalancer = healthBalancer;
			_speedBalancer = speedBalancer;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		protected virtual void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			_lootBox.Open( victimBody.position );
			Dispose();
		}

		public void Dispose()
		{
			if ( !IsAlive )
			{
				//Debug.LogError( $"Attempted to dispose a dead enemy.\n{name} | {this.GetInstanceID()}", this );
				return;
			}

			_memoryPool.Despawn( this );
		}

		public virtual void OnDespawned()
		{
			_memoryPool = null;

			_onDestroyCancelSource.Cancel();
			_onDestroyCancelSource.Dispose();
			_onDestroyCancelSource = null;

			_damageController.Died -= OnDied;
			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			_signalBus.Fire( new EnemyDestroyedSignal() { Victim = this } );
		}

		public virtual void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_memoryPool = pool;
			_onDestroyCancelSource = AppHelper.CreateLinkedCTS();

			OnLevelCycleChanged();
			Health.Replenish();

			_damageController.Died += OnDied;
			_signalBus.Subscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );

			// We set the transform's orientation so there isn't any visual blinking when moving from previous spawn position.
			transform.SetPositionAndRotation( placement.Position, placement.Rotation );
			// And we set the rigidbody's orientation so the physics engine is up to date.
			_body.position = placement.Position;
			_body.SetRotation( placement.Rotation );

			_signalBus.Fire( new EnemySpawnedSignal() { Enemy = this } );
		}

		private void OnLevelCycleChanged()
		{
			RecalibrateVelocity();

			_healthBalancer.Resolve();
		}

		public virtual void RecalibrateVelocity()
		{
			_speedBalancer.Resolve();
		}

		public virtual void StartMainBehavior()
		{
		}

		protected void FixedUpdate()
		{
			if ( !IsReady )
			{
				return;
			}

			FixedTick();
		}

		protected virtual void FixedTick()
		{

		}

		// TODO: Move into a utility?
		protected bool IsWithinShipZone( Vector2Int rowColCoord )
		{
			return rowColCoord.Row() < _levelSettings.Builder.PlayerRows;
		}

		private void OnDestroy()
		{
			if ( IsAlive )
			{
				_onDestroyCancelSource.Cancel();
				_onDestroyCancelSource.Dispose();
				_onDestroyCancelSource = null;
			}

			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
		}

		[System.Serializable]
		public class SharedSettings
		{
			public float SpawnDelay = 1;
		}

		public class Factory : PlaceholderFactory<IOrientation, EnemyController> { }
	}
}
