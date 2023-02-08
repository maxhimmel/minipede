using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.UI;
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

		private readonly GameplaySettings.Player _playerSettings;
		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly WaveController _waveController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly LevelMushroomHealer _mushroomHealer;
		private readonly ScreenFadeController _screenFadeController;

		public GameController( GameplaySettings.Player playerSettings,
			PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			WaveController waveController,
			AudioBankLoader audioBankLoader,
			LevelMushroomHealer mushroomHealer,
			ScreenFadeController screenFadeController )
		{
			_playerSettings = playerSettings;
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_waveController = waveController;
			_audioBankLoader = audioBankLoader;
			_mushroomHealer = mushroomHealer;
			_screenFadeController = screenFadeController;
		}

		public void Dispose()
		{
			_playerSpawnController.PlayerDied -= OnPlayerDead;

			_audioBankLoader.UnloadBanks().Forget();
		}

		public async void Initialize()
		{
			await _screenFadeController.FadeIn( 1 );

			// Let's wait a single frame to allow other initializables to subscribe ...
			await UniTask.Yield();

			_playerSpawnController.PlayerDied += OnPlayerDead;

			await _audioBankLoader.LoadBanks();
			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.RespawnPlayer();
			_waveController.Play().Forget();

			IsReady = true;
		}

		private async void OnPlayerDead()
		{
			_waveController.Interrupt();

			await _mushroomHealer.HealAll();
			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay );

			_playerSpawnController.RespawnPlayer();
			_waveController.Play().Forget();
		}
	}
}
