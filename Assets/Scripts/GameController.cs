using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : ILevelInitializer,
		IInitializable, 
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly IWaveController _waveController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly LevelCycleTimer _levelCycleTimer;
		private readonly SceneLoader _sceneLoader;

		public GameController( PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			IWaveController waveController,
			AudioBankLoader audioBankLoader,
			LevelCycleTimer levelCycleTimer,
			SceneLoader sceneLoader )
		{
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_waveController = waveController;
			_audioBankLoader = audioBankLoader;
			_levelCycleTimer = levelCycleTimer;
			_sceneLoader = sceneLoader;
		}

		public void Dispose()
		{
			_audioBankLoader.UnloadBanks().Forget();
		}

		public async void Initialize()
		{
			await UniTask.WaitWhile( () => _sceneLoader.IsLoading );

			await _audioBankLoader.LoadBanks();
			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.RespawnPlayer();
			_waveController.Play().Forget();
			_levelCycleTimer.Play();

			IsReady = true;
		}
	}
}
