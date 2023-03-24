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

		private bool _isReloadRequested;
		protected float _reloadEndTime;

		public AmmoProcessor( TSettings settings )
		{
			_settings = settings;
		}

		public void FireEnding()
		{
			if ( ReduceAmmo() <= 0 )
			{
				OnAmmoEmptied();
			}
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
			if ( !_isReloadRequested || IsReloading() )
			{
				return;
			}

			_isReloadRequested = false;
			ReplenishAmmo();
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