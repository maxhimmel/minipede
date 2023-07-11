using System.Collections.Generic;
using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Enemies.Spawning
{
	public class EnemySpawnWarningController
	{
		private readonly LevelGraph _levelGraph;
		private readonly SignalBus _signalBus;
		private readonly Dictionary<Vector2Int, SpawnWarningModel> _warnings;

		public EnemySpawnWarningController( LevelGraph levelGraph,
			SignalBus signalBus )
		{
			_levelGraph = levelGraph;
			_signalBus = signalBus;
			_warnings = new Dictionary<Vector2Int, SpawnWarningModel>();
		}

		public void Add( Vector2 worldPosition )
		{
			var cellCoord = _levelGraph.WorldPosToClampedCellCoord( worldPosition );

			if ( !_warnings.TryGetValue( cellCoord, out var warning ) )
			{
				warning = new SpawnWarningModel()
				{
					CellCoord = cellCoord,
					Position = _levelGraph.CellCoordToWorldPos( cellCoord ),
					Count = 0
				};

				_warnings.Add( cellCoord, warning );
			}

			++warning.Count;

			_signalBus.TryFire( new SpawnWarningChangedSignal()
			{
				Model = warning
			} );

			//Debug.Log( $"Add Warning | r:{cellCoord.Row()}, c: {cellCoord.Col()} | count: {warning.Count}" );
		}

		public void Remove( Vector2 worldPosition )
		{
			var cellCoord = _levelGraph.WorldPosToClampedCellCoord( worldPosition );

			if ( _warnings.TryGetValue( cellCoord, out var warning ) )
			{
				if ( --warning.Count <= 0 )
				{
					_warnings.Remove( cellCoord );
				}

				_signalBus.TryFire( new SpawnWarningChangedSignal()
				{
					Model = warning
				} );

				//Debug.Log( $"Remove Warning | r:{cellCoord.Row()}, c: {cellCoord.Col()} | count: {warning.Count}" );
			}
		}
	}
}