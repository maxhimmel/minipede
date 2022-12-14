using System.Collections.Generic;
using Zenject;

namespace Minipede.Gameplay
{
	public class StatusEffectController : ITickable
	{
		private readonly IDamageable _owner;
		private readonly Dictionary<System.Type, StatusEffectInvoker> _statuses;
		private readonly List<StatusEffectInvoker> _expiredStatuses;

		public StatusEffectController( IDamageable owner )
		{
			_owner = owner;
			_statuses = new Dictionary<System.Type, StatusEffectInvoker>();
			_expiredStatuses = new List<StatusEffectInvoker>();
		}

		public bool TryAdd( StatusEffectInvoker status, out StatusEffectInvoker existingStatus )
		{
			var type = status.GetType();
			if ( _statuses.TryGetValue( type, out existingStatus ) )
			{
				return false;
			}

			_statuses.Add( type, status );
			return true;
		}

		public void Remove<TStatus>()
			where TStatus : StatusEffectInvoker
		{
			_statuses.Remove( typeof( TStatus ) );
		}

		public void Clear()
		{
			_statuses.Clear();
		}

		public void Tick()
		{
			foreach ( var status in _statuses.Values )
			{
				if ( status.IsExpired )
				{
					_expiredStatuses.Add( status );
				}
				else if ( status.CanApply )
				{
					_owner.TakeDamage( status.Instigator, status.Causer, status.Settings );
				}
			}

			for ( int idx = _expiredStatuses.Count - 1; idx >= 0; --idx )
			{
				var status = _expiredStatuses[idx];
				_statuses.Remove( status.GetType() );
				_expiredStatuses.RemoveAt( idx );
			}
		}
	}
}