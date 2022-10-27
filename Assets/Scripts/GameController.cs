using System;
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
		private readonly PlayerSpawnController _playerSpawnController;
		private readonly LevelBuilder _levelBuilder;

		public GameController( GameplaySettings.Player playerSettings,
			PlayerSpawnController playerSpawnController,
			LevelBuilder levelBuilder )
		{
			_playerSettings = playerSettings;
			_playerSpawnController = playerSpawnController;
			_levelBuilder = levelBuilder;

			playerSpawnController.PlayerDied += OnPlayerDead;
		}

		public async void Initialize()
		{
			await _levelBuilder.GenerateLevel().Cancellable( AppHelper.AppQuittingToken );

			_playerSpawnController.Create();

			IsReady = true;
		}

		private async void OnPlayerDead( PlayerController deadPlayer )
		{
			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay );

			_playerSpawnController.Create();
		}

		public void Dispose()
		{
			if ( _playerSpawnController != null )
			{
				_playerSpawnController.PlayerDied -= OnPlayerDead;
			}
		}
	}
}
