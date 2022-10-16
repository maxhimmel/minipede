using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelBlockForeman
	{
		private readonly LevelGraphNavigator _levelNavigator;

		private LevelCell _currentCell;

		public LevelBlockForeman( LevelGraphNavigator levelNavigator )
		{
			_levelNavigator = levelNavigator;
		}

		public bool TryQueryFilledBlock( Vector2 worldPosition, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block == null )
			{
				instructions = null;
				return false;
			}

			instructions = new DemolishInstructions( _levelNavigator.Graph, _currentCell );
			return true;
		}

		public bool TryQueryEmptyBlock( Vector2 worldPosition, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block != null )
			{
				instructions = null;
				return false;
			}

			instructions = new RefurbishInstructions( _levelNavigator.Graph, _currentCell );
			return true;
		}

		public bool TryQueryAnyBlock( Vector2 worldPosition, out SiteInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) )
			{
				instructions = null;
				return false;
			}

			instructions = new SiteInstructions( _levelNavigator.Graph, _currentCell );
			return true;
		}

		private bool TryQueryNewBlock( Vector2 worldPosition, out LevelCell data )
		{
			if ( _levelNavigator.TryGetCellData( worldPosition, out data) && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}
	}
}
