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
		private readonly PlayerInputResolver _inputResolver;

		public PauseController( SignalBus signalBus,
			TimeController timeController,
			PlayerInputResolver inputResolver )
		{
			_signalBus = signalBus;
			_timeController = timeController;
			_inputResolver = inputResolver;
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

			var input = _inputResolver.GetInput();
			input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.UI ), true );
			input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.Gameplay ), false );
		}

		private void Resume()
		{
			_timeController.SetTimeScale( 1 );

			var input = _inputResolver.GetInput();
			input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.UI ), false );
			input.EnableMapRuleSet( nameof( ReConsts.MapEnablerRuleSet.Gameplay ), true );
		}
	}
}