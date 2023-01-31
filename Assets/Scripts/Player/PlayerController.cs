using System;
using System.Threading;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Rewired;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : IInitializable,
		IDisposable
	{
		public event System.Action<Ship> PlayerSpawned;
		public event System.Action PlayerDied;

		public CancellationToken PlayerDiedCancelToken => _playerDiedCancelSource.Token;
		public Vector2 Position => IsExploring ? _explorer.Body.position : _ship.Body.position;
		public bool IsExploring => _explorer != null;

		private readonly Rewired.Player _input;
		private readonly ShipSpawner _shipSpawner;
		private readonly ShipController _shipController;
		private readonly Explorer.Factory _explorerFactory;
		private readonly ExplorerController _explorerController;
		private readonly SignalBus _signalBus;

		private Ship _ship;
		private Explorer _explorer;
		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerController( Rewired.Player input,
			ShipSpawner shipSpawner,
			ShipController shipController,
			Explorer.Factory explorerFactory,
			ExplorerController explorerController,
			SignalBus signalBus )
		{
			_input = input;
			_shipSpawner = shipSpawner;
			_shipController = shipController;
			_explorerFactory = explorerFactory;
			_explorerController = explorerController;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();

			_input.AddButtonPressedDelegate( OnPaused, ReConsts.Action.Pause );
			_input.AddButtonPressedDelegate( OnResumed, ReConsts.Action.Resume );
		}

		public void Dispose()
		{
			_input.RemoveInputEventDelegate( OnPaused );
			_input.RemoveInputEventDelegate( OnResumed );
		}

		private void OnPaused( InputActionEventData obj )
		{
			_signalBus.Fire( new PausedSignal( isPaused: true ) );
		}

		private void OnResumed( InputActionEventData obj )
		{
			_signalBus.Fire( new PausedSignal( isPaused: false ) );
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

			_explorer = null;
			_explorerController.UnPossess();

			PlayerDied?.Invoke();
		}
	}
}
