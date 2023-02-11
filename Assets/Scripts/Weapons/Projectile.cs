using System;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class Projectile : MonoBehaviour,
		IPoolable<float, Vector2, Quaternion, IMemoryPool>,
		IDisposable
	{
		public event System.Action<Projectile> Destroyed;

		private Rigidbody2D _body;
		private SignalBus _signalBus;
		private Lifetimer _lifetimer;

		private IMemoryPool _pool;

		[Inject]
		public void Construct( Rigidbody2D body,
			SignalBus signalBus )
		{
			_body = body;
			_signalBus = signalBus;

			_lifetimer = new Lifetimer();
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

			_lifetimer.Pause();

			Destroyed?.Invoke( this );
		}

		public void OnSpawned( float lifetime, Vector2 position, Quaternion rotation, IMemoryPool pool )
		{
			_pool = pool;

			_signalBus.Subscribe<DamageDeliveredSignal>( OnDamagedOther );

			_body.position = position;
			_body.SetRotation( rotation );

			_lifetimer.StartLifetime( lifetime );
		}

		private void Update()
		{
			if ( !_lifetimer.Tick() )
			{
				Dispose();
			}
		}

		public class Factory : PlaceholderFactory<float, Vector2, Quaternion, Projectile> { }
	}
}
