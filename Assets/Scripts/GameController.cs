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
		private readonly PlayerSpawner _playerSpawner;
		private readonly LevelBuilder _levelBuilder;

		private PlayerController _player;

		public GameController( GameplaySettings.Player playerSettings,
			PlayerSpawner playerSpawner,
			LevelBuilder levelBuilder )
		{
			_playerSettings = playerSettings;
			_playerSpawner = playerSpawner;
			_levelBuilder = levelBuilder;
		}

		public async void Initialize()
		{
			await _levelBuilder.GenerateLevel();
			CreatePlayer();

			IsReady = true;
		}


		private void CreatePlayer()
		{
			_player = _playerSpawner.Spawn();
			_player.DestroyNotify.Destroyed += OnPlayerDead;
		}

		private async void OnPlayerDead( object sender, EventArgs e )
		{
			_player = null;
			await TaskHelpers.DelaySeconds( _playerSettings.RespawnDelay );

			if ( AppHelper.IsQuitting )
			{
				return;
			}

			CreatePlayer();
		}

		public void Dispose()
		{
			if ( _player != null )
			{
				_player.DestroyNotify.Destroyed -= OnPlayerDead;
			}
		}
	}
}
