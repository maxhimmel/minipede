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
		public IOrientation Orientation => IsExploring ? Explorer.Orientation : Ship != null ? Ship.Orientation : new Orientation();
		public bool IsExploring => Explorer != null;
		public Explorer Explorer { get; private set; }
		public ExplorerController ExplorerController { get; private set; }
		public Ship Ship { get; private set; }
		public ShipController ShipController { get; private set; }

		private readonly Settings _settings;
		private readonly Rewired.Player _input;
		private readonly ShipSpawner _shipSpawner;
		private readonly Explorer.Factory _explorerFactory;
		private readonly EjectModel _ejectModel;
		private readonly PauseModel _pauseModel;
		private readonly TimeController _timeController;
		private readonly SignalBus _signalBus;

		private CancellationTokenSource _playerDiedCancelSource;

		public PlayerController( Settings settings,
			Rewired.Player input,
			ShipSpawner shipSpawner,
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
			_shipSpawner = shipSpawner;
			ShipController = shipController;
			_explorerFactory = explorerFactory;
			ExplorerController = explorerController;
			_ejectModel = ejectModel;
			_pauseModel = pauseModel;
			_timeController = timeController;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			_playerDiedCancelSource = AppHelper.CreateLinkedCTS();

			_input.AddButtonPressedDelegate( OnPauseToggled, ReConsts.Action.Pause );

			_signalBus.Subscribe<IWinStateChangedSignal>( OnWinStateChanged );
		}

		public void Dispose()
		{
			if ( !_playerDiedCancelSource.IsCancellationRequested )
			{
				_playerDiedCancelSource?.Cancel();
				_playerDiedCancelSource?.Dispose();
			}

			_input.RemoveInputEventDelegate( OnPauseToggled );

			_signalBus.TryUnsubscribe<IWinStateChangedSignal>( OnWinStateChanged );
		}

		private void OnPauseToggled( InputActionEventData obj )
		{
			_pauseModel.Toggle();
		}

		private void OnShipUnpossessed()
		{
			CreateAndCacheExplorer( Ship.Orientation );
			ExplorerController.Possess( Explorer );

			Ship.AddMinimapMarker();
		}

		public void CreateAndCacheExplorer( IOrientation placement )
		{
			if ( Explorer != null )
			{
				throw new System.NotSupportedException( "Cannot have multiple explorers active." );
			}

			Explorer newExplorer = _explorerFactory.Create( placement );
			newExplorer.Died += OnExplorerDied;

			Explorer = newExplorer;
		}

		private void OnExplorerDied( Rigidbody2D victimBody, HealthController health )
		{
			var deadExplorer = Explorer;
			deadExplorer.Died -= OnExplorerDied;

			Explorer = null;
			ExplorerController.UnPossess();

			HandleGameover();
		}

		private void OnWinStateChanged( IWinStateChangedSignal signal )
		{
			if ( signal.CanWin )
			{
				ShipController.UnPossessed -= OnShipUnpossessed;
			}
		}

		public void RespawnPlayer()
		{
			if ( Ship == null )
			{
				Ship = _shipSpawner.Create();
				Ship.Died += OnShipDied;
			}
			else
			{
				Ship.Body.MovePosition( _shipSpawner.SpawnPoint.Position );
				Ship.Body.MoveRotation( 0 );
			}

			Ship.Health.Replenish();
			ShipController.Possess( Ship );
			ShipController.UnPossessed += OnShipUnpossessed;

			PlayerSpawned?.Invoke( Ship );
		}

		private void OnShipDied( Rigidbody2D victimBody, HealthController health )
		{
			_signalBus.Fire( new ShipDiedSignal() );

			HandleEjectDecision().Forget();
		}

		private async UniTaskVoid HandleEjectDecision()
		{
			_timeController.SetTimeScale( _settings.EjectSlomo );

			ShipController.UnPossessed -= OnShipUnpossessed;
			ShipController.UnPossess();

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

			_timeController.SetTimeScale( 1 );
		}

		private void HandleDeathEject()
		{
			_ejectModel.SetChoice( EjectModel.Options.Eject );

			CreateAndCacheExplorer( Ship.Orientation );
			ExplorerController.Possess( Explorer );
			Explorer.Eject( UnityEngine.Random.insideUnitCircle.normalized );

			Ship.AddMinimapMarker();
			Ship.Eject( Explorer.Body.position, PlayerDiedCancelToken ).Forget();

			ShipController.UnPossessed += OnShipUnpossessed;
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
