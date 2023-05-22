using System;
using System.Collections.Generic;
using Zenject;

namespace Minipede.Utility
{
	public class PauseController : IInitializable,
		IDisposable
	{
		private readonly PauseModel _model;
		private readonly TimeController _timeController;
		private readonly Stack<float> _timeScaleStack;

		public PauseController( PauseModel model,
			TimeController timeController )
		{
			_model = model;
			_timeController = timeController;
			_timeScaleStack = new Stack<float>();
		}

		public void Initialize()
		{
			_model.Changed += OnPaused;
		}

		public void Dispose()
		{
			_model.Changed -= OnPaused;
		}

		private void OnPaused( PauseModel model )
		{
			if ( model.IsPaused )
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
			_timeScaleStack.Push( _timeController.Scale );
			_timeController.SetTimeScale( 0 );
		}

		private void Resume()
		{
			if ( !_timeScaleStack.TryPop( out var prevScale ) )
			{
				prevScale = 1;
			}
			_timeController.SetTimeScale( prevScale );
		}
	}
}