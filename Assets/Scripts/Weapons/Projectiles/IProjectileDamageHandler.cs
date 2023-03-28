using System;

namespace Minipede.Gameplay.Weapons
{
	public interface IProjectileDamageHandler : IDisposable
	{
		void Handle( Projectile projectile, DamageDeliveredSignal signal );

		public interface ISettings
		{
			System.Type HandlerType { get; }
		}
	}
}