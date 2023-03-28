using System;
using Sirenix.OdinInspector;

namespace Minipede.Gameplay.Weapons
{
	public class ProjectilePierceHandler : IProjectileDamageHandler
	{
		private readonly Settings _settings;

		private int _pierceCount;

		public ProjectilePierceHandler( Settings settings )
		{
			_settings = settings;
		}

		public void Handle( Projectile projectile, DamageDeliveredSignal signal )
		{
			if ( _settings.IsUnlimited )
			{
				return;
			}

			if ( _pierceCount++ >= _settings.Depth )
			{
				projectile.Dispose();
			}
		}

		public void Dispose()
		{
			_pierceCount = 0;
		}

		[System.Serializable]
		public class Settings : IProjectileDamageHandler.ISettings
		{
			public Type HandlerType => typeof( ProjectilePierceHandler );

			[HorizontalGroup]
			public bool IsUnlimited;

			[HorizontalGroup]
			[MinValue( 0 ), DisableIf( "IsUnlimited" )]
			public int Depth;
		}
	}
}