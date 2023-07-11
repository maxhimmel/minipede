using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Waves
{
	public class DayNightController 
    {
		private readonly IWaveController _waveController;
		private readonly DayNightModel _dayNightModel;

		private bool _isPlaying;
		private CancellationTokenSource _updateCancelSource;

		public DayNightController( IWaveController waveController,
			DayNightModel dayNightModel )
		{
			_waveController = waveController;
			_dayNightModel = dayNightModel;
		}

		public void Play( CancellationToken cancelToken )
		{
			_isPlaying = true;

			if ( _updateCancelSource == null )
			{
				_updateCancelSource = AppHelper.CreateLinkedCTS( cancelToken );
			}

			UpdateCycle().Forget();
		}

		private async UniTaskVoid UpdateCycle()
		{
			while ( _isPlaying )
			{
				await HandleDaytime();
				await HandleNighttime();
			}
		}

		private async UniTask HandleDaytime()
		{
			_dayNightModel.SetState( isDaytime: true );
			_dayNightModel.SetProgress( 0 );

			_waveController.Play().Forget();

			await TaskHelpers.UpdateTimer( _dayNightModel.DaytimeDuration, PlayerLoopTiming.Update, _updateCancelSource.Token,
				progress =>
				{
					float daytimePercentage = _dayNightModel.DaytimeDuration / _dayNightModel.TotalDuration;
					_dayNightModel.SetProgress( daytimePercentage * progress );
				} 
			);
		}

		private async UniTask HandleNighttime()
		{
			_dayNightModel.SetState( isDaytime: false );

			_waveController.Pause();

			await TaskHelpers.UpdateTimer( _dayNightModel.NighttimeDuration, PlayerLoopTiming.Update, _updateCancelSource.Token,
				progress =>
				{
					float daytimePercentage = _dayNightModel.DaytimeDuration / _dayNightModel.TotalDuration;
					progress = Mathf.Lerp( daytimePercentage, 1, progress );
					_dayNightModel.SetProgress( progress );
				}
			);
		}

		public void Stop()
		{
			_isPlaying = false;

			if ( _updateCancelSource != null )
			{
				_updateCancelSource.Cancel();
				_updateCancelSource.Dispose();
				_updateCancelSource = null;
			}
		}
	}
}
