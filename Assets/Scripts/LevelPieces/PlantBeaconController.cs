﻿using System.Threading;
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

		public PlantBeaconController( Settings settings,
			LevelGraph levelGraph,
			BlockFactoryBus blockFactory )
		{
			_settings = settings;
			_levelGraph = levelGraph;
			_blockFactory = blockFactory;
		}

		public async UniTask<Response> PlantBeacon( Request request )
		{
			Prepare( request );
			await MoveBeaconToMushroom( request );
			await UpdateMushroomConversion( request );
			var lighthouse = CreateLighthouseWithBeacon( request );

			return new Response()
			{
				Lighthouse = lighthouse
			};
		}

		private void Prepare( Request request )
		{
			request.Explorer.ReleaseTreasure( request.Beacon );

			request.Beacon.PrepareForLighthouseEquip();
			request.Mushroom.PrepareForLighthouseConversion();
		}

		private async UniTask MoveBeaconToMushroom( Request request )
		{
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