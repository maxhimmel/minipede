using System;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Audio;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Installers;
using Minipede.Utility;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : 
		IInitializable, 
		IDisposable
	{
		public bool IsReady { get; private set; }

		private readonly GameplaySettings.Player _playerSettings;
		private readonly PlayerController _playerSpawnController;
		private readonly LevelGenerator _levelGenerator;
		private readonly EnemyWaveController _enemyWaveController;
		private readonly AudioBankLoader _audioBankLoader;
		private readonly LevelMushroomHealer _mushroomHealer;

		public GameController( GameplaySettings.Player playerSettings,
			PlayerController playerSpawnController,
			LevelGenerator levelGenerator,
			EnemyWaveController enemyWaveController,
			AudioBankLoader audioBankLoader,
			LevelMushroomHealer mushroomHealer )
		{
			_playerSettings = playerSettings;
			_playerSpawnController = playerSpawnController;
			_levelGenerator = levelGenerator;
			_enemyWaveController = enemyWaveController;
			_audioBankLoader = audioBankLoader;
			_mushroomHealer = mushroomHealer;

			playerSpawnController.PlayerDied += OnPlayerDead;
		}

		public void Dispose()
		{
			_playerSpawnController.PlayerDied -= OnPlayerDead;

			_audioBankLoader.UnloadBanks().Forget();
		}

		public async void Initialize()
		{
			// Let's wait a single frame to allow other initializables to subscribe ...
			await UniTask.Yield();

			await _audioBankLoader.LoadBanks();
			await _levelGenerator.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.RespawnPlayer();
			_enemyWaveController.Play();

			IsReady = true;
		}

		private async void OnPlayerDead()
		{
			_enemyWaveController.Interrupt();

			await _mushroomHealer.HealAll();
			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay );

			_playerSpawnController.RespawnPlayer();
			_enemyWaveController.Play();
		}
	}
}
