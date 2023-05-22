using System;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public class PoisonVolume : MonoBehaviour,
		IPoolable<Transform, Vector3, float, IMemoryPool>,
		IDisposable
	{
		private Lifetimer _lifetimer = new Lifetimer();
		private IAttack _attack;
		private ParticleSystem _vfx;

		private IMemoryPool _memoryPool;

		[Inject]
		public void Construct( IAttack attack,
			ParticleSystem vfx )
		{
			_attack = attack;
			_vfx = vfx;
		}

		public void OnSpawned( Transform owner, Vector3 position, float duration, IMemoryPool pool )
		{
			_memoryPool = pool;

			transform.position = position;

			SetOwner( owner );
			SetLifetime( duration );
		}

		public void SetOwner( Transform owner )
		{
			_attack.SetOwner( owner );
		}

		public void SetLifetime( float duration )
		{
			_lifetimer.SetDuration( duration );
		}

		public void StartExpiring()
		{
			_lifetimer.Reset();

			_vfx.time = 0;
			_vfx.Play( withChildren: true );
		}

		private void Update()
		{
			if ( !_lifetimer.Tick() )
			{
				Dispose();
			}
		}

		public void Dispose()
		{
			_memoryPool?.Despawn( this );
		}

		public void OnDespawned()
		{
			_memoryPool = null;
			_lifetimer.Pause();
		}

		public class Factory : PlaceholderFactory<Transform, Vector3, float, PoisonVolume> { }
	}
}