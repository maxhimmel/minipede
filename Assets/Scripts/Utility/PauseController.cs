using System;
using Minipede.Gameplay;
using Zenject;

namespace Minipede.Utility
{
	public class PauseController : IInitializable,
		IDisposable
	{
		private readonly SignalBus _signalBus;
		private readonly TimeController _timeController;

		public PauseController( SignalBus signalBus,
			TimeController timeController )
		{
			_signalBus = signalBus;
			_timeController = timeController;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<PausedSignal>( OnPaused );
		}

		public void Dispose()
		{
			_signalBus.Unsubscribe<PausedSignal>( OnPaused );
		}

		private void OnPaused( PausedSignal signal )
		{
			if ( signal.IsPaused )
			{
				Pause();
			}
			else
			{
				Resume();
			}
		}

		private void Pause()
		{
			_timeController.SetTimeScale( 0 );
		}

		private void Resume()
		{
			_timeController.SetTimeScale( 1 );
		}
	}
}