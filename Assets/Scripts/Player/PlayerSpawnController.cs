using System.Threading;
using Minipede.Gameplay.LevelPieces;

namespace Minipede.Gameplay.Player
{
    public class PlayerSpawnController
	{
		public event System.Action<PlayerController> PlayerSpawned;
		public event System.Action<PlayerController> PlayerDied;

		public CancellationToken PlayerDiedCancelToken { get; private set; }

		private readonly PlayerSpawner _spawner;

		private PlayerController _currentPlayer;
		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerSpawnController( PlayerSpawner spawner )
		{
			_spawner = spawner;

			_playerDiedCancelSource = new CancellationTokenSource();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;
		}

		public PlayerController Create()
		{
			if ( _currentPlayer != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple players active." );
			}

			_currentPlayer = _spawner.Create();
			_currentPlayer.Died += OnPlayerDied;

			PlayerSpawned?.Invoke( _currentPlayer );

			return _currentPlayer;
		}

		private void OnPlayerDied( UnityEngine.Rigidbody2D victimBody, HealthController health )
		{
			var deadPlayer = _currentPlayer;
			deadPlayer.Died -= OnPlayerDied;

			_playerDiedCancelSource.Cancel();
			_playerDiedCancelSource.Dispose();
			_playerDiedCancelSource = new CancellationTokenSource();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;

			_currentPlayer = null;
			PlayerDied?.Invoke( deadPlayer );
		}
	}
}
