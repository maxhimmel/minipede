using System;
using Zenject;

namespace Minipede.Utility
{
	public class PauseController : IInitializable,
		IDisposable
	{
		private readonly PauseModel _model;
		private readonly TimeController _timeController;

		public PauseController( PauseModel model,
			TimeController timeController )
		{
			_model = model;
			_timeController = timeController;
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
			_timeController.SetTimeScale( 0 );
		}

		private void Resume()
		{
			_timeController.SetTimeScale( 1 );
		}
	}
}