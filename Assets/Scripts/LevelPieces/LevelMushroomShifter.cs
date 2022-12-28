using System;
using System.Collections.Generic;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.LevelPieces
{
	public class LevelMushroomShifter : IInitializable,
		IDisposable
	{
		private readonly LevelGraph _levelGraph;
		private readonly SignalBus _signalBus;
		private readonly List<Mushroom> _mushrooms;

		private bool _ignoreNextCleanup;

		public LevelMushroomShifter( LevelGraph levelGraph,
			SignalBus signalBus )
		{
			_levelGraph = levelGraph;
			_signalBus = signalBus;

			_mushrooms = new List<Mushroom>();
		}

		public void Initialize()
		{
			_signalBus.Subscribe<BlockSpawnedSignal>( OnBlockSpawned );
			_signalBus.Subscribe<BlockDestroyedSignal>( OnBlockDestroyed );
		}

		public void Dispose()
		{
			_signalBus.TryUnsubscribe<BlockSpawnedSignal>( OnBlockSpawned );
			_signalBus.TryUnsubscribe<BlockDestroyedSignal>( OnBlockDestroyed );
		}

		private void OnBlockSpawned( BlockSpawnedSignal signal )
		{
			if ( signal.NewBlock is Mushroom newMushroom )
			{
				_mushrooms.Add( newMushroom );
			}
		}

		private void OnBlockDestroyed( BlockDestroyedSignal signal )
		{
			if ( _ignoreNextCleanup )
			{
				_ignoreNextCleanup = false;
				return;
			}

			if ( signal.Victim is Mushroom mushroom )
			{
				_mushrooms.Remove( mushroom );
			}
		}

		public void ShiftAll( Vector2Int direction )
		{
			for ( int idx = _mushrooms.Count - 1; idx >= 0; --idx )
			{
				Mushroom mushroom = _mushrooms[idx];
				mushroom.OnMoving();

				Vector2Int startCoord = _levelGraph.WorldPosToCellCoord( mushroom.transform.position );
				var startCell = _levelGraph.GetCellData( startCoord.Row(), startCoord.Col() );

				Vector2Int destCoord = startCoord + direction.ToRowCol();
				if ( _levelGraph.IsCellCoordValid( destCoord.Row(), destCoord.Col() ) )
				{
					var destCell = _levelGraph.GetCellData( destCoord.Row(), destCoord.Col() );
					destCell.Block = mushroom;
					mushroom.transform.position = destCell.Center;

					if ( startCell.Block == mushroom )
					{
						startCell.Block = null;
					}
				}
				else
				{
					if ( startCell.Block == mushroom )
					{
						startCell.Block = null;
					}

					_ignoreNextCleanup = true;
					_mushrooms.RemoveAt( idx );

					mushroom.Cleanup();
				}
			}
		}
	}
}