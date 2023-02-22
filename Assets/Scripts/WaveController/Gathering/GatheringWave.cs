using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class GatheringWave : IWave
	{
		public string Id => "Gathering";
		public bool IsRunning { get; private set; }

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;

		public GatheringWave( Settings settings,
			SignalBus signalBus )
		{
			_settings = settings;
			_signalBus = signalBus;
		}

		public void Interrupt()
		{
			IsRunning = false;
		}

		public async UniTask<IWave.Result> Play()
		{
			IsRunning = true;

			float timer = 0;
			while ( timer < _settings.Duration )
			{
				timer += Time.deltaTime;

				_signalBus.Fire( new WaveProgressSignal()
				{
					Id = Id,
					NormalizedProgress = timer / _settings.Duration
				} );

				await UniTask.Yield( PlayerLoopTiming.Update, AppHelper.AppQuittingToken );

				if ( !IsRunning )
				{
					return IWave.Result.Interrupted;
				}
			}

			IsRunning = false;

			return IWave.Result.Success;
		}

		[System.Serializable]
		public class Settings
		{
			public float Duration;
		}
	}
}