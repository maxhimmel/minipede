using Minipede.Gameplay.LevelPieces;

namespace Minipede.Gameplay.Player
{
    public class PlayerSpawnController
	{
		public event System.Action<PlayerController> PlayerSpawned;
		public event System.Action<PlayerController> PlayerDied;

		private readonly PlayerSpawner _spawner;

		private PlayerController _currentPlayer;

		public PlayerSpawnController( PlayerSpawner spawner )
		{
			_spawner = spawner;
		}

		public PlayerController Create()
		{
			if ( _currentPlayer != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple players active." );
			}

			_currentPlayer = _spawner.Create();
			_currentPlayer.DestroyNotify.Destroyed += OnPlayerDead;

			PlayerSpawned?.Invoke( _currentPlayer );

			return _currentPlayer;
		}

		private void OnPlayerDead( object sender, System.EventArgs e )
		{
			var deadPlayer = _currentPlayer;
			deadPlayer.DestroyNotify.Destroyed -= OnPlayerDead;

			_currentPlayer = null;
			PlayerDied?.Invoke( deadPlayer );
		}
	}
}
