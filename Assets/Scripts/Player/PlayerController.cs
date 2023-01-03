using System.Threading;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Player
{
    public class PlayerController
	{
		public event System.Action<Ship> PlayerSpawned;
		public event System.Action<Ship> ShipDied;
		public event System.Action<Explorer> ExplorerDied;

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

			_playerDiedCancelSource = new CancellationTokenSource();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;
		}

		public Ship CreateShip()
		{
			if ( _ship != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple ships active." );
			}

			_ship = _shipSpawner.Create();
			_ship.Died += OnShipDied;

			_shipController.Possess( _ship );

			PlayerSpawned?.Invoke( _ship );

			return _ship;
		}

		private void OnShipDied( Rigidbody2D victimBody, HealthController health )
		{
			var deadShip = _ship;
			deadShip.Died -= OnShipDied;

			_playerDiedCancelSource.Cancel();
			_playerDiedCancelSource.Dispose();
			_playerDiedCancelSource = new CancellationTokenSource();
			PlayerDiedCancelToken = _playerDiedCancelSource.Token;

			_ship = null;
			_shipController.UnPossess();
			ShipDied?.Invoke( deadShip );
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
			ExplorerDied?.Invoke( deadExplorer );

			// Self-destruct explorer's ship ...
			_ship.TakeDamage( deadExplorer.transform, deadExplorer.transform, KillInvoker.Kill );
		}

		public IOrientation GetOrientation()
		{
			if ( _ship == null && _explorer == null )
			{
				return new Orientation();
			}

			return IsExploring
				? _explorer.Orientation
				: _ship.Orientation;
		}
	}
}
