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
		IPlayerPawnLocator,
		IInitializable,
		IDisposable
	{
		public event System.Action<Ship> PlayerSpawned;
		public event System.Action PlayerDied;

		public CancellationToken PlayerDiedCancelToken => _playerDiedCancelSource.Token;
		public IOrientation Orientation => IsExploring ? _explorer.Orientation : _ship != null ? _ship.Orientation : new Orientation();
		public bool IsExploring => _explorer != null;

		private readonly Settings _settings;
		private readonly Rewired.Player _input;
		private readonly ShipController _shipController;
		private readonly Explorer.Factory _explorerFactory;
		private readonly ExplorerController _explorerController;
		private readonly EjectModel _ejectModel;
		private readonly PauseModel _pauseModel;
		private readonly TimeController _timeController;
		private readonly SignalBus _signalBus;

		private Ship _ship;
		private Explorer _explorer;
		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerController( Settings settings,
			Rewired.Player input,
			ShipController shipController,
			Explorer.Factory explorerFactory,
			ExplorerController explorerController,
			EjectModel ejectModel,
			PauseModel pauseModel,
			TimeController timeController,
			SignalBus signalBus )
		{
			_settings = settings;
			_input = input;
			_shipController = shipController;
			_explorerFactory = explorerFactory;
			_explorerController = explorerController;
			_ejectModel = ejectModel;
			_pauseModel = pauseModel;
			_timeController = timeController;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();

			_input.AddButtonPressedDelegate( OnPauseToggled, ReConsts.Action.Pause );
		}

		public void Dispose()
		{
			if ( !_playerDiedCancelSource.IsCancellationRequested )
			{
				_playerDiedCancelSource?.Cancel();
				_playerDiedCancelSource?.Dispose();
			}

			_input.RemoveInputEventDelegate( OnPauseToggled );
		}

		private void OnPauseToggled( InputActionEventData obj )
		{
			_pauseModel.Toggle();
		}

		private Explorer CreateExplorer()
		{
			if ( _explorer != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple explorers active." );
			}

			Explorer newExplorer = _explorerFactory.Create( _ship.Orientation );
			newExplorer.Died += OnExplorerDied;

			return newExplorer;
		}

		private void OnExplorerDied( Rigidbody2D victimBody, HealthController health )
		{
			var deadExplorer = _explorer;
			deadExplorer.Died -= OnExplorerDied;

			_explorer = null;
			_explorerController.UnPossess();

			HandleGameover();
		}

		public void TakeOverSpawningProcess( Ship ship )
		{
			_ship = ship;
			_ship.Died += OnShipDied;

			_ship.Health.Replenish();
			_shipController.Possess( _ship );
			_shipController.ExitedShip += OnExplorerExitedShip;

			PlayerSpawned?.Invoke( _ship );
		}

		private void OnExplorerExitedShip( Ship ship )
		{
			_explorer = CreateExplorer();
			_explorerController.Possess( _explorer );

			ship.AddMinimapMarker();
		}

		private void OnShipDied( Rigidbody2D victimBody, HealthController health )
		{
			_signalBus.Fire( new ShipDiedSignal() );

			HandleEjectDecision().Forget();
		}

		private async UniTaskVoid HandleEjectDecision()
		{
			_timeController.SetTimeScale( _settings.EjectSlomo );

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
				else
				{
					_ejectModel.UpdateCountdown();
					if ( _ejectModel.Countdown <= 0 )
					{
						HandleGameover();
					}
				}
			}

			_ejectModel.Reset();

			_timeController.SetTimeScale( 1 );
		}

		private void HandleDeathEject()
		{
			_ejectModel.SetChoice( EjectModel.Options.Eject );

			_explorer = CreateExplorer();
			_explorerController.Possess( _explorer );
			_explorer.Eject( UnityEngine.Random.insideUnitCircle.normalized );

			_ship.AddMinimapMarker();
			_ship.PlayParkingAnimation();
			_ship.Eject( _explorer.Body.position, PlayerDiedCancelToken ).Forget();
		}

		private void HandleGameover()
		{
			Dispose();

			_ejectModel.SetChoice( EjectModel.Options.Die );

			PlayerDied?.Invoke();
		}

		[System.Serializable]
		public class Settings
		{
			[Range( 0, 1 )]
			public float EjectSlomo;
		}
	}
}
