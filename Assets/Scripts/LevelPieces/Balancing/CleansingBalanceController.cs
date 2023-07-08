using System;
using System.Collections.Generic;
using Minipede.Utility;
using Sirenix.OdinInspector;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class CleansingBalanceController : IInitializable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly LevelBalanceController _balanceController;
		private readonly SignalBus _signalBus;

		private int _activatedEventCount;

		public CleansingBalanceController( Settings settings,
			LevelBalanceController balanceController,
			SignalBus signalBus )
		{
			_settings = settings;
			_balanceController = balanceController;
			_signalBus = signalBus;
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		public void Initialize()
		{
			_signalBus.TrySubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );

			_settings.CleansingEvents.Sort();
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			while ( TryGetNextEvent( out var nextEvent ) && nextEvent.Cleansing <= signal.NormalizedLevel )
			{
				++_activatedEventCount;
				_balanceController.SetCycle( nextEvent.Cycle );
			}
		}

		private bool TryGetNextEvent( out CleansingToCycle nextEvent )
		{
			bool hasEvents = _settings.CleansingEvents != null && _activatedEventCount < _settings.CleansingEvents.Count;

			nextEvent = hasEvents
				? _settings.CleansingEvents[_activatedEventCount]
				: null;

			return nextEvent != null;
		}

		[System.Serializable]
		public class Settings
		{
			[TableList]
			public List<CleansingToCycle> CleansingEvents;
		}

		[System.Serializable]
		public class CleansingToCycle : IComparable<CleansingToCycle>
		{
			[PropertyRange( 0, 1 )]
			public float Cleansing;
			[MinValue( 0 )]
			public int Cycle;

			public int CompareTo( CleansingToCycle other )
			{
				if ( Cleansing < other.Cleansing )
				{
					return -1;
				}
				else if ( Cleansing > other.Cleansing )
				{
					return 1;
				}
				return 0;
			}
		}
	}
}
