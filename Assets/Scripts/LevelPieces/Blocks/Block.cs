using Minipede.Installers;
using Minipede.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class Block : MonoBehaviour,
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

		protected Rigidbody2D _body;
		private IDamageController _damageController;
		private LevelGraph _levelGraph;
		private SignalBus _signalBus;

		private IMemoryPool _pool;

		[Inject]
		public void Construct( Rigidbody2D body,
			IDamageController damageController,
			LevelGraph levelGraph,
			SignalBus signalBus )
		{
			_body = body;
			_damageController = damageController;
			_levelGraph = levelGraph;
			_signalBus = signalBus;
		}

		public int TakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
			OnTakeDamage( instigator, causer, data );
			return _damageController.TakeDamage( instigator, causer, data );
		}

		protected virtual void OnTakeDamage( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
		{
		}

		protected virtual void HandleDeath( Rigidbody2D victimBody, HealthController health )
		{
			Dispose();
		}

		public void Dispose()
		{
			if ( _pool != null )
			{
				_pool.Despawn( this );
			}
			else
			{
				OnDespawned();
			}
		}

		public void OnDespawned()
		{
			_pool = null;

			_damageController.Died -= HandleDeath;

			_levelGraph.RemoveBlock( this );

			_signalBus.TryFire( new BlockDestroyedSignal() { Victim = this } );
		}

		public void OnSpawned( IOrientation placement, IMemoryPool pool )
		{
			_pool = pool;

			Health.Replenish();
			_damageController.Died += HandleDeath;

			_body.transform.position = placement.Position;

			_signalBus.TryFire( new BlockSpawnedSignal()
			{
				NewBlock = this
			} );
		}

		public class Factory : UnityPrefabFactory<Block>
		{
			public Factory( DiContainer container )
				: base( container )
			{
			}
		}
	}
}
