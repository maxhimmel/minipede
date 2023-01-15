using System.Threading;
using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
    public class PlayerController
	{
		public event System.Action<Ship> PlayerSpawned;
		public event System.Action PlayerDied;

		public CancellationToken PlayerDiedCancelToken { get; private set; }
		public bool IsExploring => _explorer != null;

		private readonly ShipSpawner _shipSpawner;
		private readonly ShipController _shipController;
		private readonly Explorer.Factory _explorerFactory;
		private readonly ExplorerController _explorerController;

		private Ship _ship;
		private Explorer _explorer;
		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerController( ShipSpawner spawner,
			ShipController shipController,
			Explorer.Factory explorerFactory,
			ExplorerController explorerController )
		{
			_shipSpawner = spawner;
			_shipController = shipController;
			_explorerFactory = explorerFactory;
			_explorerController = explorerController;

			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;
		}

		public void RespawnPlayer()
		{
			if ( _ship == null )
			{
				_ship = _shipSpawner.Create();
				_ship.Died += OnShipDied;
			}
			else
			{
				_ship.Body.MovePosition( _shipSpawner.SpawnPoint.Position );
				_ship.Body.MoveRotation( 0 );
			}

			_shipController.Possess( _ship );

			PlayerSpawned?.Invoke( _ship );
		}

		private void OnShipDied( Rigidbody2D victimBody, HealthController health )
		{
			var deadShip = _ship;
			deadShip.Died -= OnShipDied;

			_playerDiedCancelSource.Cancel();
			_playerDiedCancelSource.Dispose();
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;

			_ship = null;
			_shipController.UnPossess();

			PlayerDied?.Invoke();
		}

		public Explorer CreateExplorer()
		{
			if ( _explorer != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple explorers active." );
			}

			_explorer = _explorerFactory.Create( _ship.Orientation );
			_explorer.Died += OnExplorerDied;

			_explorerController.Possess( _explorer );

			return _explorer;
		}

		private void OnExplorerDied( Rigidbody2D victimBody, HealthController health )
		{
			var deadExplorer = _explorer;
			deadExplorer.Died -= OnExplorerDied;

			_playerDiedCancelSource.Cancel();
			_playerDiedCancelSource.Dispose();
			_playerDiedCancelSource = new CancellationTokenSource();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;

			_explorer = null;
			_explorerController.UnPossess();

			PlayerDied?.Invoke();
		}
	}
}
