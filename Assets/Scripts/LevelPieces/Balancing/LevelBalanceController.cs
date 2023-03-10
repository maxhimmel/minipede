using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelBalanceController
	{
		public virtual int Cycle { get; protected set; }

		private readonly SignalBus _signalBus;

		public LevelBalanceController( Settings settings,
			SignalBus signalBus )
		{
			Cycle = settings.StartCycle;

			_signalBus = signalBus;
		}

		public virtual void IncrementCycle()
		{
			_signalBus.Fire( new LevelCycleChangedSignal( ++Cycle ) );
			Debug.Log( $"<color=orange>Level cycle incremented:</color> (<b>{Cycle}</b>)" );
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 0 )]
			public int StartCycle;
		}
	}
}