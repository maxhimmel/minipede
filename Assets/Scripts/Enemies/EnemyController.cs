using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies
{
	public class EnemyController : MonoBehaviour,
		IDamageController
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
		public Rigidbody2D Body => _body;

		protected Rigidbody2D _body;
		private IDamageController _damageController;
		protected GameController _gameController;
		protected LevelGraph _levelGraph;
		protected LevelForeman _levelForeman;

		private CancellationTokenSource _onDestroyCancelSource;
		protected CancellationToken _onDestroyCancelToken;

		[Inject]
		public void Construct( Rigidbody2D body,
			IDamageController damageController,
			GameController gameController, 
			LevelGraph levelGraph,
			LevelForeman foreman )
		{
			_body = body;
			_damageController = damageController;
			_gameController = gameController;
			_levelGraph = levelGraph;
			_levelForeman = foreman;

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
			_damageController.Died -= OnDied;
			Destroy( gameObject );
		}

		protected void OnDestroy()
		{
			_onDestroyCancelSource.Cancel();

			if ( _damageController != null )
			{
				_damageController.Damaged -= OnDamaged;
				_damageController.Died -= OnDied;
			}
		}

		protected async void Start()
		{
			OnStart();

			while ( !IsReady )
			{
				await UniTask.WaitForFixedUpdate();
			}

			OnReady();
		}

		protected virtual void OnStart()
		{

		}

		protected virtual void OnReady()
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
