using System;
using System.Collections.Generic;
using Minipede.Gameplay.Enemies.Spawning;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class EnemySpawnWarningWidget : IInitializable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly EnemySpawnMarkerFactoryBus _markerFactory;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<Vector2Int, EnemySpawnMarker> _markers;

		public EnemySpawnWarningWidget( Settings settings,
			EnemySpawnMarkerFactoryBus markerFactory,
			SignalBus signalBus )
		{
			_settings = settings;
			_markerFactory = markerFactory;
			_signalBus = signalBus;

			_markers = new Dictionary<Vector2Int, EnemySpawnMarker>();
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<SpawnWarningChangedSignal>( OnSpawnWarningChanged );
		}

		public void Initialize()
		{
			_signalBus.TrySubscribe<SpawnWarningChangedSignal>( OnSpawnWarningChanged );
		}

		private void OnSpawnWarningChanged( SpawnWarningChangedSignal signal )
		{
			var model = signal.Model;

			if ( !_markers.TryGetValue( model.CellCoord, out var marker ) )
			{
				marker = _markerFactory.Create( _settings.MarkerPrefab, new Orientation( model.Position ) );
				_markers.Add( model.CellCoord, marker );
			}

			marker.SetCount( model.Count );

			if ( model.Count <= 0 )
			{
				marker.Dispose();
				_markers.Remove( model.CellCoord );
			}
		}

		[System.Serializable]
		public class Settings
		{
			public EnemySpawnMarker MarkerPrefab;
		}
	}
}
