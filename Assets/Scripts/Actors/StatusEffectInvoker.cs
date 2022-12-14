using UnityEngine;

namespace Minipede.Gameplay
{
	public abstract class StatusEffectInvoker : IDamageInvoker
	{
		public ISettings Settings { get; }
		public bool CanApply => _nextApplyTime <= Time.timeSinceLevelLoad;
		public bool IsExpired => Settings.CanExpire && _expirationTime <= Time.timeSinceLevelLoad;

		public Transform Instigator { get; private set; }
		public Transform Causer { get; private set; }

		protected readonly StatusEffectController _statusEffectController;

		private float _expirationTime;
		private float _nextApplyTime;

		public StatusEffectInvoker( ISettings settings,
			StatusEffectController statusEffectController )
		{
			Settings = settings;
			_statusEffectController = statusEffectController;

			_expirationTime = Time.timeSinceLevelLoad + settings.Duration;
		}

		public DamageResult Invoke( IDamageable damageable, Transform instigator, Transform causer )
		{
			if ( !_statusEffectController.TryAdd( this, out var existingStatus ) && existingStatus.CanApply )
			{
				return existingStatus.Apply( damageable, instigator, causer );
			}
			else
			{
				return this.Apply( damageable, instigator, causer );
			}
		}

		protected DamageResult Apply( IDamageable damageable, Transform instigator, Transform causer )
		{
			Instigator = instigator;
			Causer = causer;

			_nextApplyTime = Time.timeSinceLevelLoad + Settings.ApplyRate;

			return InternalInvoke( damageable, instigator, causer );
		}

		protected abstract DamageResult InternalInvoke( IDamageable damageable, Transform instigator, Transform causer );

		protected TSettings GetSettings<TSettings>()
			where TSettings : ISettings
		{
			return (TSettings)Settings;
		}

		public interface ISettings : IDamageInvoker.ISettings
		{
			float ApplyRate { get; }
			bool CanExpire { get; }
			float Duration { get; }
		}
	}
}