using System;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public partial class Projectile : MonoBehaviour,
		IPoolable<Vector2, Quaternion, IMemoryPool>,
		IDisposable,
		ICleanup
	{
		public event System.Action<Projectile> Destroyed;

		private Rigidbody2D _body;
		private SignalBus _signalBus;

		private IMemoryPool _pool;

		[Inject]
		public void Construct( Rigidbody2D body,
			SignalBus signalBus )
		{
			_body = body;
			_signalBus = signalBus;
		}

		public void Launch( Vector2 impulse )
		{
			Launch( impulse, 0 );
		}

		public void Launch( Vector2 impulse, float torque )
		{
			if ( impulse != Vector2.zero )
			{
				_body.AddForce( impulse, ForceMode2D.Impulse );
			}
			if ( torque != 0 )
			{
				_body.AddTorque( torque, ForceMode2D.Impulse );
			}
		}

		public void OnDamagedOther( DamageDeliveredSignal message )
		{
			//GameObject.Destroy( Body.gameObject );
			Cleanup();
		}

		public void Cleanup()
		{
			Dispose();
		}

		public void Dispose()
		{
			_pool?.Despawn( this );
		}

		public void OnDespawned()
		{
			_pool = null;

			_signalBus.Unsubscribe<DamageDeliveredSignal>( OnDamagedOther );

			_body.velocity = Vector2.zero;
			_body.angularVelocity = 0;

			Destroyed?.Invoke( this );
		}

		public void OnSpawned( Vector2 position, Quaternion rotation, IMemoryPool pool )
		{
			_pool = pool;

			_signalBus.Subscribe<DamageDeliveredSignal>( OnDamagedOther );

			_body.position = position;
			_body.SetRotation( rotation );
		}
	}
}
