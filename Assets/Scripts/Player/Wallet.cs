using System.Collections.Generic;
using Minipede.Gameplay.Treasures;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class Wallet
    {
		private readonly SignalBus _signalBus;
		private readonly Dictionary<System.Type, int> _treasures;

		public Wallet( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_treasures = new Dictionary<System.Type, int>();
		}

		public void CollectTreasure( Treasure treasure )
		{
			var treasureType = treasure.GetType();
			if ( !_treasures.TryGetValue( treasureType, out int amount ) )
			{
				_treasures.Add( treasureType, 0 );
			}

			int newAmount = amount + 1;
			_treasures[treasureType] = newAmount;

			_signalBus.TryFire( new CollectedTreasureSignal()
			{
				TreasureType = treasureType,
				TotalAmount = newAmount
			} );
		}
    }
}
