using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.Weapons
{
	public class LimitProjectilesSafety : IFireSafety
	{
		[HideLabel]
		[SerializeField] private Settings _settings;

		private readonly HashSet<Projectile> _livingProjectiles = new HashSet<Projectile>();

		public bool CanFire()
		{
			return _livingProjectiles.Count < _settings.MaxProjectiles;
		}

		public void Notify( Projectile firedProjectile )
		{
			firedProjectile.Destroyed += OnProjectileDestroyed;
			_livingProjectiles.Add( firedProjectile );
		}

		private void OnProjectileDestroyed( Projectile projectile )
		{
			projectile.Destroyed -= OnProjectileDestroyed;
			if ( !_livingProjectiles.Remove( projectile ) )
			{
				throw new System.DataMisalignedException();
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public int MaxProjectiles;
		}
	}
}
