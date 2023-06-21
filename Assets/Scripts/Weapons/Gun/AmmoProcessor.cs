using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Weapons
{
	public abstract class AmmoProcessor<TProcessor, TSettings> : 
		IAmmoHandler,
		IFixedTickable
		where TProcessor : AmmoProcessor<TProcessor, TSettings>
		where TSettings : AmmoProcessor<TProcessor, TSettings>.Settings
	{
		public virtual event Action Emptied;

		public abstract AmmoData AmmoData { get; }

		protected readonly TSettings _settings;
		private readonly SignalBus _signalBus;

		private bool _isReloadRequested;
		protected float _reloadEndTime;

		public AmmoProcessor( TSettings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;
		}

		public void FireEnding()
		{
			int remainingAmmo = ReduceAmmo();

			FireAmmoSignal();

			if ( remainingAmmo <= 0 )
			{
				OnAmmoEmptied();
			}
		}

		protected void FireAmmoSignal()
		{
			_signalBus.TryFire( new AmmoStateSignal()
			{
				NormalizedAmmo = AmmoData.Normalized
			} );
		}

		/// <returns>Remaining ammo count.</returns>
		protected abstract int ReduceAmmo();

		protected void OnAmmoEmptied()
		{
			Emptied?.Invoke();

			if ( _settings.AutoReload )
			{
				Reload();
			}
		}

		public virtual void Reload()
		{
			_isReloadRequested = true;
			_reloadEndTime = Time.timeSinceLevelLoad + _settings.ReloadDuration;
		}

		public virtual void FixedTick()
		{
			if ( !_isReloadRequested )
			{
				return;
			}
			else if ( IsReloading() )
			{
				FireReloadTimeSignal();
				return;
			}

			_isReloadRequested = false;
			FireReloadTimeSignal();

			ReplenishAmmo();
			FireAmmoSignal();
		}

		public bool CanFire()
		{
			return !IsReloading() && HasAmmo();
		}

		private bool IsReloading()
		{
			return _reloadEndTime > Time.timeSinceLevelLoad;
		}

		protected abstract bool HasAmmo();

		protected void FireReloadTimeSignal()
		{
			float remainingTime = _reloadEndTime - Time.timeSinceLevelLoad;

			_signalBus.TryFire( new ReloadStateSignal()
			{
				NormalizedTimer = Mathf.Clamp01( 1 - remainingTime / _settings.ReloadDuration )
			} );
		}

		protected abstract void ReplenishAmmo();

		public abstract class Settings : IGunModule
		{
			public Type ModuleType => typeof( TProcessor );

			[BoxGroup( "Reloading", ShowLabel = false )]
			
			[HorizontalGroup( "Reloading/Main" )]
			public bool AutoReload;
			
			[HorizontalGroup( "Reloading/Main" )]
			[MinValue( 0 )]
			public float ReloadDuration;
		}
	}
}