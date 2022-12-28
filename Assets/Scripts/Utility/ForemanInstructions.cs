using Minipede.Gameplay;
using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelForeman
	{
		public abstract class InternalInstructions
		{
			public LevelCell Cell => _currentCell;

			protected readonly LevelGraph _levelGraph;
			protected readonly LevelCell _currentCell;
			protected readonly MushroomProvider _mushroomProvider;

			public InternalInstructions( LevelGraph levelGraph,
				LevelCell currentCell,
				MushroomProvider mushroomProvider )
			{
				_levelGraph = levelGraph;
				_currentCell = currentCell;
				_mushroomProvider = mushroomProvider;
			}

			//public bool IsBlockOfType( Block.Type type )
			//{
			//	var block = _currentCell.Block;
			//	return block.name.Contains( type.ToString() );
			//}
		}

		public class DemolishInstructions : InternalInstructions
		{
			public DemolishInstructions( LevelGraph levelGraph, LevelCell currentCell, MushroomProvider mushroomProvider ) 
				: base( levelGraph, currentCell, mushroomProvider )
			{
			}

			/// <summary>
			/// Simple removal of a block.
			/// </summary>
			public RefurbishInstructions Destroy()
			{
				_currentCell.Block.Cleanup();
				return new RefurbishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}

			/// <summary>
			/// Removal of a block through the <see cref="IDamageable"/> API.
			/// </summary>
			public RefurbishInstructions Kill( Transform instigator, Transform causer )
			{
				_currentCell.Block.TakeDamage( instigator, causer, KillInvoker.Kill );
				return new RefurbishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}
		}

		public class RefurbishInstructions : InternalInstructions
		{
			public RefurbishInstructions( LevelGraph levelGraph, LevelCell currentCell, MushroomProvider mushroomProvider )
				: base( levelGraph, currentCell, mushroomProvider )
			{
			}

			public DemolishInstructions CreateStandardMushroom()
			{
				var mushroomPrefab = _mushroomProvider.GetStandardAsset();
				_levelGraph.CreateBlock( mushroomPrefab, _currentCell.CellCoord.Row(), _currentCell.CellCoord.Col() );

				return new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}

			public DemolishInstructions CreatePoisonMushroom()
			{
				var mushroomPrefab = _mushroomProvider.GetPoisonAsset();
				_levelGraph.CreateBlock( mushroomPrefab, _currentCell.CellCoord.Row(), _currentCell.CellCoord.Col() );

				return new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}

			public DemolishInstructions CreateFlowerMushroom()
			{
				var mushroomPrefab = _mushroomProvider.GetFlowerAsset();
				_levelGraph.CreateBlock( mushroomPrefab, _currentCell.CellCoord.Row(), _currentCell.CellCoord.Col() );

				return new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}
		}

		public class AllInstructions : InternalInstructions
		{
			public bool IsEmpty => !IsFilled;
			public bool IsFilled => _currentCell.Block != null;

			public AllInstructions( LevelGraph levelGraph, LevelCell currentCell, MushroomProvider mushroomProvider )
				: base( levelGraph, currentCell, mushroomProvider )
			{
			}

			public DemolishInstructions Demolish()
			{
				Debug.Assert( IsFilled, $"Cannot create demolish instructions for a cell that isn't filled." );
				return new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}

			public RefurbishInstructions Refurbish()
			{
				Debug.Assert( IsEmpty, $"Cannot create refurbish instructions for a cell that isn't empty." );
				return new RefurbishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			}
		}
	}
}
