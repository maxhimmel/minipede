using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Treasures;
using Minipede.Gameplay.Fx;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public abstract class EnemyController : MonoBehaviour,
		IDamageController,
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

		public bool IsReady => _gameController.IsReady;
		public bool IsAlive => !_onDestroyCancelToken.IsCancellationRequested;
		public Rigidbody2D Body => _body;

		protected Rigidbody2D _body;
		private IDamageController _damageController;
		protected GameController _gameController;
		protected LevelGraph _levelGraph;
		protected LevelForeman _levelForeman;
		protected SignalBus _signalBus;
		private LootBox _lootBox;

		private CancellationTokenSource _onDestroyCancelSource;
		protected CancellationToken _onDestroyCancelToken;

		[Inject]
		public void Construct( Rigidbody2D body,
			IDamageController damageController,
			GameController gameController, 
			LevelGraph levelGraph,
			LevelForeman foreman,
			SignalBus signalBus,
			LootBox lootBox )
		{
			_body = body;
			_damageController = damageController;
			_gameController = gameController;
			_levelGraph = levelGraph;
			_levelForeman = foreman;
			_signalBus = signalBus;
			_lootBox = lootBox;

			_onDestroyCancelSource = new CancellationTokenSource();
			_onDestroyCancelToken = _onDestroyCancelSource.Token;

			damageController.Damaged += OnDamaged;
			damageController.Died += OnDied;
		}

		public int TakeDamage( Transform instigator, Transform causer, DamageDatum data )
		{
			return _damageController.TakeDamage( instigator, causer, data );
		}

		protected virtual void OnDamaged( Rigidbody2D victimBody, HealthController health )
		{
		}

		protected virtual void OnDied( Rigidbody2D victimBody, HealthController health )
		{
			_lootBox.Open( victimBody.position );
			Cleanup();
		}

		public void Cleanup()
		{
			if ( !IsAlive )
			{
				return;
			}

			_onDestroyCancelSource.Cancel();
			_onDestroyCancelSource.Dispose();

			if ( _damageController != null )
			{
				_damageController.Damaged -= OnDamaged;
				_damageController.Died -= OnDied;
			}

			_signalBus.Fire( new EnemyDestroyedSignal() { Victim = this } );

			Destroy( gameObject );
		}

		protected async void Start()
		{
			OnStart();

			while ( !IsReady )
			{
				await UniTask.WaitForFixedUpdate( _onDestroyCancelToken );
			}

			OnReady();
		}

		protected virtual void OnStart()
		{
			_signalBus.Fire( new EnemySpawnedSignal() { Enemy = this } );
		}

		protected virtual void OnReady()
		{

		}

		public virtual void OnSpawned()
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
	}
}
