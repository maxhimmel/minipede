﻿using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelBalanceController
	{
		public virtual int Cycle { get; protected set; }

		protected readonly SignalBus _signalBus;

		public LevelBalanceController( Settings settings,
			SignalBus signalBus )
		{
			Cycle = settings.StartCycle;

			_signalBus = signalBus;
		}

		public virtual void IncrementCycle()
		{
			SetCycle( Cycle + 1 );
		}

		public virtual void SetCycle( int cycle )
		{
			Cycle = cycle;
			_signalBus.Fire( new LevelCycleChangedSignal( Cycle ) );
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