using System;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class Projectile : MonoBehaviour,
		IPoolable<Projectile.Settings, Vector2, Quaternion, IMemoryPool>,
		IDisposable
	{
		public event System.Action<Projectile> Destroyed;

		private Rigidbody2D _body;
		private IDamageDealer _damageDealer;
		private Lifetimer _lifetimer;

		private IMemoryPool _pool;

		[Inject]
		public void Construct( Rigidbody2D body,
			IDamageDealer damagerDealer )
		{
			_body = body;
			_damageDealer = damagerDealer;

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

		private void OnDamagedOther( DamageDeliveredSignal message )
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

			_damageDealer.DamageDelivered -= OnDamagedOther;
			_damageDealer.SetOwner( null );
			_damageDealer.SetDamage( null );

			_body.velocity = Vector2.zero;
			_body.angularVelocity = 0;

			_lifetimer.Pause();

			Destroyed?.Invoke( this );
		}

		public void OnSpawned( Settings settings, Vector2 position, Quaternion rotation, IMemoryPool pool )
		{
			_pool = pool;

			_damageDealer.DamageDelivered += OnDamagedOther;
			_damageDealer.SetOwner( settings.Owner );
			_damageDealer.SetDamage( settings.Damage );

			_body.position = position;
			_body.SetRotation( rotation );

			_lifetimer.StartLifetime( settings.Lifetime );
		}

		private void Update()
		{
			if ( !_lifetimer.Tick() )
			{
				Dispose();
			}
		}

		public class Settings
		{
			public float Lifetime;
			public Transform Owner;
			public DamageTrigger.Settings Damage;

			public Settings() { }
			public Settings( float lifetime, Transform owner, DamageTrigger.Settings damage )
			{
				Lifetime = lifetime;
				Owner = owner;
				Damage = damage;
			}
		}

		public class Factory : PlaceholderFactory<Settings, Vector2, Quaternion, Projectile> { }
	}
}
