using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Waves;
using Minipede.Installers;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : ILevelInitializer,
		IInitializable, 
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly PlayerSettings.Player _playerSettings;
		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly IWaveController _waveController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly LevelCycleTimer _levelCycleTimer;
		private readonly LevelMushroomHealer _mushroomHealer;
		private readonly NighttimeController _nighttimeController;
		private readonly SceneLoader _sceneLoader;

		public GameController( PlayerSettings.Player playerSettings,
			PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			IWaveController waveController,
			AudioBankLoader audioBankLoader,
			LevelCycleTimer levelCycleTimer,
			LevelMushroomHealer mushroomHealer,
			NighttimeController nighttimeController,
			SceneLoader sceneLoader )
		{
			_playerSettings = playerSettings;
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_waveController = waveController;
			_audioBankLoader = audioBankLoader;
			_levelCycleTimer = levelCycleTimer;
			_mushroomHealer = mushroomHealer;
			_nighttimeController = nighttimeController;
			_sceneLoader = sceneLoader;
		}

		public void Dispose()
		{
			_playerSpawnController.PlayerDied -= OnPlayerDead;

			_audioBankLoader.UnloadBanks().Forget();
		}

		public async void Initialize()
		{
			await UniTask.WaitWhile( () => _sceneLoader.IsLoading );

			_playerSpawnController.PlayerDied += OnPlayerDead;

			await _audioBankLoader.LoadBanks();
			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.RespawnPlayer();
			_waveController.Play().Forget();
			_levelCycleTimer.Play();

			IsReady = true;
		}

		private async void OnPlayerDead()
		{
			if ( !_nighttimeController.IsNighttime )
			{
				_waveController.Interrupt();
				_levelCycleTimer.Stop();

				await _mushroomHealer.HealAll();
			}

			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay, AppHelper.AppQuittingToken );
			_playerSpawnController.RespawnPlayer();

			if ( !_nighttimeController.IsNighttime )
			{
				_waveController.Play().Forget();
				_levelCycleTimer.Play();
			}
		}
	}
}
