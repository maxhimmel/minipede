using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelForeman
	{
		private readonly LevelGraph _levelGraph;
		private readonly MushroomProvider _mushroomProvider;

		private LevelCell _currentCell;

		public LevelForeman( LevelGraph levelGraph,
			MushroomProvider mushroomProvider )
		{
			_levelGraph = levelGraph;
			_mushroomProvider = mushroomProvider;
		}

		public void ClearQuery()
		{
			_currentCell = null;
		}



		public bool TryQueryFilledBlock( Vector2 worldPosition, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block == null )
			{
				instructions = null;
				return false;
			}

			instructions = new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}

		public bool TryQueryFilledBlock( Vector2Int cellCoord, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out var cellData ) || cellData.Block == null )
			{
				instructions = null;
				return false;
			}

			instructions = new DemolishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}



		public bool TryQueryEmptyBlock( Vector2 worldPosition, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block != null )
			{
				instructions = null;
				return false;
			}

			instructions = new RefurbishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}

		public bool TryQueryEmptyBlock( Vector2Int cellCoord, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out var cellData ) || cellData.Block != null )
			{
				instructions = null;
				return false;
			}

			instructions = new RefurbishInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}



		public bool TryQueryAnyBlock( Vector2 worldPosition, out AllInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out _ ) )
			{
				instructions = null;
				return false;
			}

			instructions = new AllInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}

		public bool TryQueryAnyBlock( Vector2Int cellCoord, out AllInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out _ ) )
			{
				instructions = null;
				return false;
			}

			instructions = new AllInstructions( _levelGraph, _currentCell, _mushroomProvider );
			return true;
		}



		private bool TryQueryNewBlock( Vector2 worldPosition, out LevelCell data )
		{
			if ( _levelGraph.TryGetCellData( worldPosition, out data ) && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}

		private bool TryQueryNewBlock( Vector2Int cellCoord, out LevelCell data )
		{
			data = _levelGraph.GetCellData( cellCoord.Row(), cellCoord.Col() );
			if ( data != null && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}
	}
}
