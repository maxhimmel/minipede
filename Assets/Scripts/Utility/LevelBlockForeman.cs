using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelBlockForeman
	{
		private readonly LevelGraph _level;

		private LevelCell _currentCell;

		public LevelBlockForeman( LevelGraph level )
		{
			_level = level;
		}

		public bool TryQueryFilledBlock( Vector2 worldPosition, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block == null )
			{
				instructions = null;
				return false;
			}

			instructions = new DemolishInstructions( _level, _currentCell );
			return true;
		}

		public bool TryQueryEmptyBlock( Vector2 worldPosition, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block != null )
			{
				instructions = null;
				return false;
			}

			instructions = new RefurbishInstructions( _level, _currentCell );
			return true;
		}

		public bool TryQueryAnyBlock( Vector2 worldPosition, out SiteInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) )
			{
				instructions = null;
				return false;
			}

			instructions = new SiteInstructions( _level, _currentCell );
			return true;
		}

		private bool TryQueryNewBlock( Vector2 worldPosition, out LevelCell data )
		{
			if ( _level.TryGetCellData( worldPosition, out data) && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}
	}
}
