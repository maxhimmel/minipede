using System.Threading;
using Cysharp.Threading.Tasks;
using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	public class PlantBeaconController
	{
		private readonly Settings _settings;
		private readonly LevelGraph _levelGraph;
		private readonly BlockFactoryBus _blockFactory;
		private readonly CleansedArea.Factory _cleansedAreaFactory;

		public PlantBeaconController( Settings settings,
			LevelGraph levelGraph,
			BlockFactoryBus blockFactory,
			CleansedArea.Factory cleansedAreaFactory )
		{
			_settings = settings;
			_levelGraph = levelGraph;
			_blockFactory = blockFactory;
			_cleansedAreaFactory = cleansedAreaFactory;
		}

		public async UniTask<Response> PlantBeacon( Request request )
		{
			Prepare( request );
			var cleansedArea = ActiveNewCleansedArea( request );
			
			await MoveBeaconToMushroom( request );
			await UpdateMushroomConversion( request );

			var lighthouse = CreateLighthouseWithBeacon( request );
			cleansedArea.PlayFillAnimation();

			return new Response()
			{
				CleansedArea = cleansedArea,
				Lighthouse = lighthouse
			};
		}

		private void Prepare( Request request )
		{
			request.Explorer.ReleaseTreasure( request.Beacon );

			request.Beacon.PrepareForLighthouseEquip();
			request.Mushroom.PrepareForLighthouseConversion();
		}

		private CleansedArea ActiveNewCleansedArea( Request request )
		{
			var cleansedAreaPrefab = request.Beacon.CleansedAreaProvider.GetAsset();
			if ( cleansedAreaPrefab == null )
			{
				return null;
			}

			var newArea = _cleansedAreaFactory.Create( 
				cleansedAreaPrefab, 
				new Orientation( request.Mushroom.transform.position ) 
			);

			newArea.Activate();
			return newArea;
		}

		private async UniTask MoveBeaconToMushroom( Request request )
		{
			request.Beacon.PlayPlantAnimation( request.Mushroom );

			await request.Beacon.SnapToPosition( 
				request.Mushroom.transform.position, 
				_settings.BeaconSnapDuration, 
				_settings.BeaconSnapCurve,
				request.CancelToken
			);
		}

		private async UniTask UpdateMushroomConversion( Request request )
		{
			request.Mushroom.PlayConvertToLighthouseAnimation();

			if ( _settings.ConvertToLighthousePause > 0 )
			{
				await TaskHelpers.DelaySeconds( _settings.ConvertToLighthousePause, request.CancelToken );
			}

			request.Mushroom.Dispose();
		}

		private Lighthouse CreateLighthouseWithBeacon( Request request )
		{
			var mushroomPosition = request.Mushroom.transform.position;

			Lighthouse newLighthouse = request.SnapToGrid
				? _levelGraph.CreateBlock( request.LighthousePrefab, mushroomPosition )
				: (Lighthouse)_blockFactory.Create( request.LighthousePrefab, new Orientation( mushroomPosition ) );

			newLighthouse.Equip( request.Beacon );

			return newLighthouse;
		}

		public class Request
		{
			public Explorer Explorer;
			public Beacon Beacon;
			public Mushroom Mushroom;
			public Lighthouse LighthousePrefab;
			public bool SnapToGrid;
			public CancellationToken CancelToken;
		}

		public class Response
		{
			public CleansedArea CleansedArea;
			public Lighthouse Lighthouse;
		}

		[System.Serializable]
		public class Settings
		{
			public float ConvertToLighthousePause = 0.5f;

			[Space]
			public float BeaconSnapDuration;
			public AnimationCurve BeaconSnapCurve = AnimationCurve.EaseInOut( 0, 0, 1, 1 );
		}
	}
}