using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Wallet
    {
		private readonly SignalBus _signalBus;
		private readonly Dictionary<ResourceType, int> _treasures;

		public Wallet( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_treasures = new Dictionary<ResourceType, int>();
		}

		/// <returns>Total amount collected.</returns>
		public int Collect( ResourceType resource )
		{
			if ( !_treasures.TryGetValue( resource, out int amount ) )
			{
				_treasures.Add( resource, 0 );
			}

			int newAmount = amount + 1;
			_treasures[resource] = newAmount;

			_signalBus.TryFire( new ResourceAmountChangedSignal()
			{
				ResourceType = resource,
				TotalAmount = newAmount
			} );

			return newAmount;
		}

		/// <returns>Total amount remaining.</returns>
		public int Spend( ResourceType resource, int spendAmount )
		{
			if ( !_treasures.TryGetValue( resource, out int total ) )
			{
				throw new System.NotSupportedException( $"Cannot spend <b>{resource.name}</b> " +
					$"since none have been collected." );
			}
			if ( spendAmount > total )
			{
				throw new System.NotSupportedException( $"Cannot spend <b>{spendAmount} {resource.name}(s)</b> " +
					$"when the total amount is <b>{total}</b>." );
			}

			int newAmount = total - spendAmount;
			_treasures[resource] = newAmount;

			_signalBus.TryFire( new ResourceAmountChangedSignal()
			{
				ResourceType = resource,
				TotalAmount = newAmount
			} );

			return newAmount;
		}
	}
}
