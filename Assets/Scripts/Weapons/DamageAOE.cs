using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
    public class DamageAOE : MonoBehaviour,
		IAttack
	{
		private Settings _settings;
		private Dictionary<IDamageable, DamageTick> _damageables;
		private List<IDamageable> _expiredDamageables;

		private Transform _owner;

		[Inject]
		public void Construct( Settings settings )
		{
			_settings = settings;

			_damageables = new Dictionary<IDamageable, DamageTick>();
			_expiredDamageables = new List<IDamageable>();
		}

		public void SetOwner( Transform owner )
		{
			_owner = owner;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( !TryGetDamageable( collision, out var damageable ) )
			{
				return;
			}

			if ( !_damageables.ContainsKey( damageable ) )
			{
				_damageables.Add( damageable, new DamageTick( damageable, _settings.Rate ) );
			}
		}

		private void OnTriggerExit2D( Collider2D collision )
		{
			if ( TryGetDamageable( collision, out var damageable ) )
			{
				_damageables.Remove( damageable );
			}
		}

		private bool TryGetDamageable( Collider2D collision, out IDamageable damageable )
		{
			var body = collision.attachedRigidbody;
			damageable = body?.GetComponent<IDamageable>();
			if ( damageable == null || !damageable.Health.IsAlive )
			{
				return false;
			}

			return _settings.Damage.IsHittable( body );
		}

		private void Update()
		{
			if ( _damageables.Count <= 0 )
			{
				return;
			}

			foreach ( var dmgTick in _damageables.Values )
			{
				bool isLiving = dmgTick.Tick( _owner, transform, _settings.Damage.Type );
				if ( !isLiving )
				{
					_expiredDamageables.Add( dmgTick.Victim );
				}
			}

			for ( int idx = _expiredDamageables.Count - 1; idx >= 0; --idx )
			{
				var victim = _expiredDamageables[idx];
				_damageables.Remove( victim );
				_expiredDamageables.RemoveAt( idx );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public float Rate;

			[BoxGroup, HideLabel]
			public DamageTrigger.Settings Damage;
		}

		private class DamageTick
		{
			public IDamageable Victim { get; }

			private readonly float _rate;

			private float _nextDamageTime;

			public DamageTick( IDamageable victim,
				float rate )
			{
				Victim = victim;
				_rate = rate;
			}

			/// <returns>True while the victim is still alive.</returns>
			public bool Tick( Transform instigator, Transform causer, IDamageInvoker.ISettings data )
			{
				if ( _nextDamageTime <= Time.timeSinceLevelLoad )
				{
					_nextDamageTime = Time.timeSinceLevelLoad + _rate;
					Victim?.TakeDamage( instigator, causer, data );
				}

				return Victim != null && Victim.Health.IsAlive;
			}
		}
	}
}
