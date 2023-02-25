using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class StartGameController : ILevelInitializer,
		IInitializable,
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly LevelGenerator _levelGenerator;
		private readonly IWaveController _waveController;
		private readonly IPlayerLifetimeHandler _playerController;

		public StartGameController( LevelGenerator levelGenerator,
			IWaveController waveController,
			IPlayerLifetimeHandler playerController )
		{
			_levelGenerator = levelGenerator;
			_waveController = waveController;
			_playerController = playerController;
		}

		public async void Initialize()
		{
			await _levelGenerator.GenerateLevel()
				.Cancellable( _playerController.PlayerDiedCancelToken );

			_waveController.Play()
				.Cancellable( _playerController.PlayerDiedCancelToken )
				.Forget();

			IsReady = true;
		}

		public void Dispose()
		{
			_waveController.Interrupt();
		}
	}
}
