using System;
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
		private readonly LevelBuilder _levelBuilder;
		private readonly EnemyWaveController _enemyWaveController;

		public GameController( GameplaySettings.Player playerSettings,
			PlayerController playerSpawnController,
			LevelBuilder levelBuilder,
			EnemyWaveController enemyWaveController )
		{
			_playerSettings = playerSettings;
			_playerSpawnController = playerSpawnController;
			_levelBuilder = levelBuilder;
			_enemyWaveController = enemyWaveController;

			playerSpawnController.ShipDied += OnPlayerDead;
		}

		public async void Initialize()
		{
			await _levelBuilder.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.CreateShip();
			_enemyWaveController.Play();

			IsReady = true;
		}

		private async void OnPlayerDead( Ship deadPlayer )
		{
			_enemyWaveController.Interrupt();

			await _levelBuilder.HealBlocks();
			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay );

			_playerSpawnController.CreateShip();
			_enemyWaveController.Play();
		}

		public void Dispose()
		{
			if ( _playerSpawnController != null )
			{
				_playerSpawnController.ShipDied -= OnPlayerDead;
			}
		}
	}
}
