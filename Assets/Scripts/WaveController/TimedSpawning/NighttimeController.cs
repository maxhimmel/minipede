using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Waves
{
	public class NighttimeController : IInitializable,
		IDisposable
	{
		public bool IsNighttime { get; private set; }

		private readonly Settings _settings;
		private readonly SignalBus _signalBus;
		private readonly LevelCycleTimer _cycleTimer;
		private readonly IWaveController _waveController;
		private readonly IPlayerLifetimeHandler _playerLifetime;

		public NighttimeController( Settings settings,
			SignalBus signalBus,
			LevelCycleTimer cycleTimer,
			IWaveController waveController,
			IPlayerLifetimeHandler playerLifetime )
		{
			_settings = settings;
			_signalBus = signalBus;
			_cycleTimer = cycleTimer;
			_waveController = waveController;
			_playerLifetime = playerLifetime;
		}

		public void Initialize()
		{
			_signalBus.Subscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
		}

		private void OnLevelCycleChanged( LevelCycleChangedSignal signal )
		{
			bool isNighttime = signal.Cycle % _settings.CycleRate == 0;
			if ( isNighttime )
			{
				Debug.Log( $"<color=purple>Nighttime!</color> @ Cycle: {signal.Cycle}" );

				HandleNighttime().Forget();
			}
		}

		private async UniTaskVoid HandleNighttime()
		{
			if ( IsNighttime )
			{
				throw new System.NotSupportedException( $"Cannot transition into nighttime when it's already nighttime." );
			}

			IsNighttime = true;
			_cycleTimer.Stop();
			_waveController.Pause();

			float timer = 0;
			while ( timer < 1 )
			{
				timer += Time.deltaTime / _settings.Duration;

				_signalBus.TryFire( new NighttimeStateChangedSignal()
				{
					IsNighttime = true,
					NormalizedProgress = Mathf.Clamp01( timer )
				} );

				await UniTask.Yield( PlayerLoopTiming.Update, _playerLifetime.PlayerDiedCancelToken );
				if ( _playerLifetime.PlayerDiedCancelToken.IsCancellationRequested )
				{
					return;
				}
			}

			_waveController.Play().Forget();
			_cycleTimer.Play();
			IsNighttime = false;

			_signalBus.TryFire( new NighttimeStateChangedSignal()
			{
				IsNighttime = false,
				NormalizedProgress = 1
			} );
		}

		[System.Serializable]
		public class Settings
		{
			[MinValue( 1 )]
			public int CycleRate;

			[MinValue( 0 )]
			public float Duration;
		}
	}
}
