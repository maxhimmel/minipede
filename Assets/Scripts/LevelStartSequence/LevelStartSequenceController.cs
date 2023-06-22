using System.Threading;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.StartSequence
{
	public class LevelStartSequenceController
    {
		private readonly Settings _settings;
		private readonly PlayerController _playerController;
		private readonly ArenaBoundary _arenaBoundary;
		private readonly Explorer.Factory _explorerFactory;
		private readonly ShipSpawner _shipSpawner;
		private readonly BeaconFactoryBus _beaconFactory;
		private readonly BlockFactoryBus _blockFactory;
		private readonly SignalBus _signalBus;
		private readonly CinemachineSmoothPath _plantPath;
		private readonly CinemachineSmoothPath _shipPath;
		private readonly CleansedArea _startCleansedArea;

		private Mushroom _lighthouseMushroom;

		public LevelStartSequenceController( Settings settings,
			PlayerController playerController,
			ArenaBoundary arenaBoundary,
			Explorer.Factory explorerFactory,
			ShipSpawner shipSpawner,
			BeaconFactoryBus beaconFactory,
			BlockFactoryBus blockFactory,
			SignalBus signalBus,
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
			_signalBus = signalBus;
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
			_arenaBoundary.SetCollisionActive( false );

			await TaskHelpers.DelaySeconds( _settings.StartDelay, cancelToken );

			// Create explorer ...
			var explorer = _explorerFactory.Create( _settings.ExplorerSpawnPosition );

			// Spawn a beacon for the explorer to drag ...
			var beaconSpawnPos = new Orientation( _settings.ExplorerSpawnPosition + Vector2.right );
			var beacon = _beaconFactory.Create( _settings.StartBeaconType, beaconSpawnPos );

			// Grab the beacon ...
			explorer.StartGrabbing();
			await UniTask.Yield( PlayerLoopTiming.LastPostLateUpdate, cancelToken );
			explorer.StopGrabbing();

			// Move towards mushroom for beacon planting ...
			await MoveAlongPath( explorer, _plantPath, cancelToken );

			// Replace specially placed mushroom w/lighthouse ...
			GameObject.Destroy( _lighthouseMushroom.gameObject );
			var lighthouse = (Lighthouse)_blockFactory.Create(
				_settings.LighthousePrefab,
				_lighthouseMushroom.Orientation
			);

			// Plant beacon into lighthouse ...
			explorer.ReleaseTreasure( beacon );
			lighthouse.Equip( beacon );
			_startCleansedArea.Activate();

			_signalBus.TryFire( new StartingAreaCleansedSignal() );

			// Wait for cleansing area to fill up ...
			await TaskHelpers.DelaySeconds( _settings.CleansingPauseDuration, cancelToken );

			// TODO: VFX for creating ship ...
			// ...

			// Spawn ship ...
			var ship = _shipSpawner.Create();
			ship.PlaySpawnAnimation();

			// TODO: Hide action glyphs during "selection" process ...
			// ...

			// Move towards ship ...
			await MoveAlongPath( explorer, _shipPath, cancelToken );

			// Enter and control ship ...
			explorer.EnterShip( ship );
			_playerController.TakeOverSpawningProcess( ship );

			_arenaBoundary.SetCollisionActive( true );
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
			[BoxGroup( "Animation" )]
			public float StartDelay = 1;
			[BoxGroup( "Animation" ), Space]
			public float ExplorerSpeed = 4.5f;
			[BoxGroup( "Animation" )]
			public float ArrivalDelay = 0.5f;
			[BoxGroup( "Animation" )]
			public float CleansingPauseDuration = 1f;

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
