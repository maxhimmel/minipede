using System.Threading;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;
using Zenject;
using Minipede.Installers;
using System;

namespace Minipede.Gameplay.Enemies
{
	public class EnemyController : MonoBehaviour,
		IDamageController,
		IPoolable<IOrientation, IMemoryPool>,
		IDisposable,
		ICleanup
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
		public bool IsReady => _gameController.IsReady;
		public bool IsAlive => _onDestroyCancelSource != null && !OnDestroyCancelToken.IsCancellationRequested;
		public CancellationToken OnDestroyCancelToken => _onDestroyCancelSource.Token;
		public Rigidbody2D Body => _body;

		protected Rigidbody2D _body;
		private IDamageController _damageController;
		protected GameController _gameController;
		protected LevelGraph _levelGraph;
		protected LevelForeman _levelForeman;
		protected SignalBus _signalBus;
		private LootBox _lootBox;
		private GameplaySettings.Level _levelSettings;

		private CancellationTokenSource _onDestroyCancelSource;
		private IMemoryPool _memoryPool;

		[Inject]
		public void Construct( Rigidbody2D body,
			IDamageController damageController,
			GameController gameController, 
			LevelGraph levelGraph,
			LevelForeman foreman,
			SignalBus signalBus,
			LootBox lootBox,
			GameplaySettings.Level levelSettings )
		{
			_body = body;
			_damageController = damageController;
			_gameController = gameController;
			_levelGraph = levelGraph;
			_levelForeman = foreman;
			_signalBus = signalBus;
			_lootBox = lootBox;
			_levelSettings = levelSettings;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		protected virtual void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			_lootBox.Open( victimBody.position );
			Cleanup();
		}

		public void Cleanup()
		{
			Dispose();
		}

		public void Dispose()
		{
			_memoryPool.Despawn( this );
		}

		public virtual void OnDespawned()
		{
			_memoryPool = null;

			_onDestroyCancelSource.Cancel();
			_onDestroyCancelSource.Dispose();
			_onDestroyCancelSource = null;

			_damageController.Died -= OnDied;

			_signalBus.Fire( new EnemyDestroyedSignal() { Victim = this } );
		}

		public virtual void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_memoryPool = pool;

			_onDestroyCancelSource = AppHelper.CreateLinkedCTS();

			Health.Replenish();

			_damageController.Died += OnDied;

			// We set the transform's orientation so there isn't any visual blinking when moving from previous spawn position.
			transform.SetPositionAndRotation( placement.Position, placement.Rotation );
			// And we set the rigidbody's orientation so the physics engine is up to date.
			_body.position = placement.Position;
			_body.SetRotation( placement.Rotation );

			_signalBus.Fire( new EnemySpawnedSignal() { Enemy = this } );
		}

		public virtual void OnSpawned()
		{
		}

		public virtual void RecalibrateVelocity()
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

		public class Factory : PlaceholderFactory<IOrientation, EnemyController> { }
	}
}
