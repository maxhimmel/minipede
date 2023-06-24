using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Cameras;
using Minipede.Gameplay.Cutscene;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.StartSequence
{
	public class LevelStartSequenceController : ILevelStartSequence
    {
		private readonly Settings _settings;
		private readonly PlayerController _playerController;
		private readonly ArenaBoundary _arenaBoundary;
		private readonly Explorer.Factory _explorerFactory;
		private readonly ShipSpawner _shipSpawner;
		private readonly BeaconFactoryBus _beaconFactory;
		private readonly BlockFactoryBus _blockFactory;
		private readonly ICameraToggler _cameraToggler;
		private readonly SignalBus _signalBus;
		private readonly CutsceneModel _cutsceneModel;
		private readonly CinemachineSmoothPath _plantPath;
		private readonly CinemachineSmoothPath _shipPath;
		private readonly CleansedArea _startCleansedArea;

		private CancellationTokenSource _skipCancelSource;
		private Mushroom _lighthouseMushroom;
		private Explorer _explorer;
		private Beacon _beacon;
		private Lighthouse _lighthouse;
		private Ship _ship;

		public LevelStartSequenceController( Settings settings,
			PlayerController playerController,
			ArenaBoundary arenaBoundary,
			Explorer.Factory explorerFactory,
			ShipSpawner shipSpawner,
			BeaconFactoryBus beaconFactory,
			BlockFactoryBus blockFactory,
			ICameraToggler cameraToggler,
			SignalBus signalBus,
			CutsceneModel cutsceneModel,
			[Inject( Id = "Path_PlantPosition" )] CinemachineSmoothPath plantPath, 
			[Inject( Id = "Path_ShipPosition" )] CinemachineSmoothPath shipPath, 
			[Inject( Id = "CleansedArea_Start" )] CleansedArea startCleansedArea )
		{
			_settings = settings;
			_playerController = playerController;
			_arenaBoundary = arenaBoundary;
			_explorerFactory = explorerFactory;
			_shipSpawner = shipSpawner;
			_beaconFactory = beaconFactory;
			_blockFactory = blockFactory;
			_cameraToggler = cameraToggler;
			_signalBus = signalBus;
			_cutsceneModel = cutsceneModel;
			_plantPath = plantPath;
			_shipPath = shipPath;
			_startCleansedArea = startCleansedArea;
		}

		public void CreateLighthouseMushrooms()
		{
			_lighthouseMushroom = (Mushroom)_blockFactory.Create(
				_settings.MushroomPrefab,
				new Orientation( _settings.LighthouseMushroomPosition )
			);
		}

		public async UniTask Play( CancellationToken cancelToken )
		{
			_cutsceneModel.SetPlayState( true );
			_cutsceneModel.PlayStateChanged += OnCutsceneSkipped;

			_skipCancelSource = AppHelper.CreateLinkedCTS( cancelToken );

			await UpdateSequence( _skipCancelSource.Token );

			_skipCancelSource.Dispose();
			_skipCancelSource = null;
		}

		private void OnCutsceneSkipped( CutsceneModel model )
		{
			if ( model.IsPlaying )
			{
				return;
			}

			_skipCancelSource.Cancel();
			_skipCancelSource.Dispose();
			_skipCancelSource = null;

			Dispose();
		}

		public void Dispose()
		{
			if ( _ship == null )
			{
				SpawnShip();
			}
			if ( _beacon == null )
			{
				SpawnBeacon();
			}
			if ( _lighthouseMushroom != null )
			{
				PlantBeacon();
			}
			if ( _explorer != null )
			{
				_explorer.Dispose();
			}

			_playerController.TakeOverSpawningProcess( _ship );

			EndSequence();
		}

		private async UniTask UpdateSequence( CancellationToken cancelToken )
		{
			await StartSequence( cancelToken );

			SpawnExplorer();
			SpawnBeacon();

			await ExplorerGrabBeacon( cancelToken );
			await MoveAlongPath( _explorer, _plantPath, cancelToken );

			PlantBeacon();
			await TaskHelpers.DelaySeconds( _settings.CleansingPauseDuration, cancelToken );

			SpawnShip();
			await TaskHelpers.DelaySeconds( _settings.ShipCreatedPauseDuration, cancelToken );

			// TODO: Hide action glyphs during "selection" process ...
				// ...

			await MoveAlongPath( _explorer, _shipPath, cancelToken );

			await ExplorerPilotShip( cancelToken );
			EndSequence();
		}

		private async UniTask StartSequence( CancellationToken cancelToken )
		{
			//_cameraToggler.Activate(); // No need to activate since it starts activated within the prefab ...
			_arenaBoundary.SetCollisionActive( false );

			await TaskHelpers.DelaySeconds( _settings.StartDelay, cancelToken );
		}

		private void SpawnExplorer()
		{
			_explorer = _explorerFactory.Create( _settings.ExplorerSpawnPosition );
		}

		private void SpawnBeacon()
		{
			var beaconSpawnPos = new Orientation( _settings.ExplorerSpawnPosition + Vector2.right );
			_beacon = _beaconFactory.Create( _settings.StartBeaconType, beaconSpawnPos );
		}

		private async UniTask ExplorerGrabBeacon( CancellationToken cancelToken )
		{
			_explorer.StartGrabbing();
			{
				await UniTask.Yield( PlayerLoopTiming.LastPostLateUpdate, cancelToken );
			}
			_explorer.StopGrabbing();
		}

		private void PlantBeacon()
		{
			// Replace mushroom w/lighthouse ...
			GameObject.Destroy( _lighthouseMushroom.gameObject );
			_lighthouseMushroom = null;

			_lighthouse = (Lighthouse)_blockFactory.Create(
				_settings.LighthousePrefab,
				new Orientation( _settings.LighthouseMushroomPosition )
			);

			// Plant beacon into lighthouse ...
			_explorer?.ReleaseTreasure( _beacon );
			_lighthouse.Equip( _beacon );

			// Activate cleansed area ...
			bool isSkipped = _skipCancelSource == null;
			if ( isSkipped )
			{
				_startCleansedArea.ImmediateFillCleansedArea();
			}
			else
			{
				_startCleansedArea.Activate();
			}

			_signalBus.TryFire( new StartingAreaCleansedSignal()
			{
				IsSkipped = isSkipped
			} );
		}

		private void SpawnShip()
		{
			// TODO: VFX for creating ship ...
			// ...

			_ship = _shipSpawner.Create();
			_ship.SetActionGlyphsActive( false );
			_ship.PlaySpawnAnimation();
		}

		private async UniTask ExplorerPilotShip( CancellationToken cancelToken )
		{
			_explorer.EnterShip( _ship );
			_explorer = null;

			if ( _settings.PilotShipPauseDuration > 0 )
			{
				await TaskHelpers.DelaySeconds( _settings.PilotShipPauseDuration, cancelToken );
			}

			_playerController.TakeOverSpawningProcess( _ship );
		}

		private void EndSequence()
		{
			_signalBus.TryFire( new HUDOnlineSignal() );

			_ship.SetActionGlyphsActive( true );

			_arenaBoundary.SetCollisionActive( true );
			_cameraToggler.Deactivate();

			_cutsceneModel.PlayStateChanged -= OnCutsceneSkipped;
			_cutsceneModel.SetPlayState( false );
		}

		private async UniTask MoveAlongPath( Explorer explorer, CinemachineSmoothPath path, CancellationToken cancelToken )
		{
			// Determine how long it'll take for explorer to travel the path ...
			float PathLength = path.PathLength;
			float pathDuration = PathLength / _settings.ExplorerSpeed;
			float pathLerp = 0;

			// Move explorer along path ...
			while ( pathLerp < 1 )
			{
				pathLerp += Time.deltaTime / pathDuration;

				var newPos = path.EvaluatePositionAtUnit( pathLerp, CinemachinePathBase.PositionUnits.Normalized );
				Quaternion newRot = Quaternion.Euler( 0, 0, 180 ) * path.EvaluateOrientationAtUnit( pathLerp, CinemachinePathBase.PositionUnits.Normalized );

				explorer.Body.MovePosition( newPos );
				explorer.Body.SetRotation( newRot );

				await UniTask.Yield( PlayerLoopTiming.Update, cancelToken );
			}

			await TaskHelpers.DelaySeconds( _settings.ArrivalDelay, cancelToken );
		}

		[System.Serializable]
		public class Settings
		{
			[BoxGroup]
			public float SkipHoldDuration = 1;

			[BoxGroup( "Animation" )]
			public float StartDelay = 1;

			[BoxGroup( "Animation" ), Space]
			public float ExplorerSpeed = 4.5f;
			[BoxGroup( "Animation" )]
			public float ArrivalDelay = 0.5f;
			[BoxGroup( "Animation" )]
			public float CleansingPauseDuration = 1f;
			[BoxGroup( "Animation" )]
			public float ShipCreatedPauseDuration = 1f;
			[BoxGroup( "Animation" )]
			public float PilotShipPauseDuration = 0.3f;

			[BoxGroup( "Animation" ), Space, HideLabel]
			public CameraToggler.Settings Camera;

			[BoxGroup( "Lighthouse Generation" )]
			public ResourceType StartBeaconType;
			[BoxGroup( "Lighthouse Generation" )]
			public Lighthouse LighthousePrefab;
			[BoxGroup( "Lighthouse Generation" )]
			public Mushroom MushroomPrefab;

			[BoxGroup( "Placements" )]
			public Vector2 LighthouseMushroomPosition;
			[BoxGroup( "Placements" )]
			public Vector2 ExplorerSpawnPosition;
		}
	}
}
