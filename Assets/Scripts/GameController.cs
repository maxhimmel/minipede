using System;
using System.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay
{
	public class GameController : IInitializable, IDisposable
	{
		private readonly Settings _settings;
		private readonly PlayerSpawner _playerSpawner;
		private readonly LevelGraph _levelGraph;

		private PlayerController _player;

		public GameController( Settings settings,
			PlayerSpawner playerSpawner,
			LevelGraph levelGraph )
		{
			_settings = settings;
			_playerSpawner = playerSpawner;
			_levelGraph = levelGraph;
		}

		public async void Initialize()
		{
			await _levelGraph.GenerateLevel();
			CreatePlayer();
		}

		private void CreatePlayer()
		{
			_player = _playerSpawner.Spawn();
			_player.DestroyNotify.Destroyed += OnPlayerDead;
		}

		private async void OnPlayerDead( object sender, EventArgs e )
		{
			_player = null;
			await TaskHelpers.DelaySeconds( _settings.PlayerRespawnRate );

			CreatePlayer();
		}

		public void Dispose()
		{
			if ( _player != null )
			{
				_player.DestroyNotify.Destroyed -= OnPlayerDead;
			}
		}

		[System.Serializable]
		public struct Settings
		{
			public float PlayerRespawnRate;
		}
	}
}
