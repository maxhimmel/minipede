using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Utility;
using Rewired;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Player
{
    public class PlayerController : IPlayerLifetimeHandler,
		IInitializable,
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
		private readonly EjectModel _ejectModel;
		private readonly SignalBus _signalBus;

		private Ship _ship;
		private Explorer _explorer;
		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerController( Rewired.Player input,
			ShipSpawner shipSpawner,
			ShipController shipController,
			Explorer.Factory explorerFactory,
			ExplorerController explorerController,
			EjectModel ejectModel,
			SignalBus signalBus )
		{
			_input = input;
			_shipSpawner = shipSpawner;
			_shipController = shipController;
			_explorerFactory = explorerFactory;
			_explorerController = explorerController;
			_ejectModel = ejectModel;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();

			_input.AddButtonPressedDelegate( OnPaused, ReConsts.Action.Pause );
			_input.AddButtonPressedDelegate( OnResumed, ReConsts.Action.Resume );

			_signalBus.Subscribe<IWinStateChangedSignal>( OnWinStateChanged );
		}

		public void Dispose()
		{
			_playerDiedCancelSource.Cancel();

			_input.RemoveInputEventDelegate( OnPaused );
			_input.RemoveInputEventDelegate( OnResumed );

			_signalBus.Unsubscribe<IWinStateChangedSignal>( OnWinStateChanged );
		}

		private void OnPaused( InputActionEventData obj )
		{
			_signalBus.Fire( new PausedSignal( isPaused: true ) );
		}

		private void OnResumed( InputActionEventData obj )
		{
			_signalBus.Fire( new PausedSignal( isPaused: false ) );
		}

		private void OnShipUnpossessed()
		{
			CreateExplorer();
		}

		private Explorer CreateExplorer()
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
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();

			_explorer = null;
			_explorerController.UnPossess();

			PlayerDied?.Invoke();
		}

		private void OnWinStateChanged( IWinStateChangedSignal signal )
		{
			if ( signal.CanWin )
			{
				_shipController.UnPossessed -= OnShipUnpossessed;
			}
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

			_ship.Health.Replenish();
			_shipController.Possess( _ship );
			_shipController.UnPossessed += OnShipUnpossessed;

			PlayerSpawned?.Invoke( _ship );
		}

		private void OnShipDied( Rigidbody2D victimBody, HealthController health )
		{
			_signalBus.Fire( new ShipDiedSignal() );

			HandleEjectDecision().Forget();
		}

		private async UniTaskVoid HandleEjectDecision()
		{
			_shipController.UnPossessed -= OnShipUnpossessed;
			_shipController.UnPossess();

			while ( !_ejectModel.Choice.HasValue )
			{
				await UniTask.Delay( 0, ignoreTimeScale: true, cancellationToken: PlayerDiedCancelToken );

				if ( _input.GetButtonDown( ReConsts.Action.Eject ) )
				{
					HandleDeathEject();
				}
				else if ( _input.GetButtonDown( ReConsts.Action.Die ) )
				{
					HandleGameover();
				}

				if ( !_ejectModel.Choice.HasValue )
				{
					_ejectModel.UpdateCountdown();
					if ( _ejectModel.Countdown <= 0 )
					{
						HandleGameover();
					}
				}
			}

			_ejectModel.Reset();
		}

		private void HandleDeathEject()
		{
			_ejectModel.SetChoice( EjectModel.Options.Eject );

			CreateExplorer()
				.Eject( UnityEngine.Random.insideUnitCircle );

			_ship.Eject();

			_shipController.UnPossessed += OnShipUnpossessed;
		}

		private void HandleGameover()
		{
			_ejectModel.SetChoice( EjectModel.Options.Die );

			PlayerDied?.Invoke();
		}
	}
}
