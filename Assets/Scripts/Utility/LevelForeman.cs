using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelForeman
	{
		private readonly LevelBuilder _builder;

		private LevelCell _currentCell;

		public LevelForeman( LevelBuilder levelBuilder )
		{
			_builder = levelBuilder;
		}

		public bool TryQueryFilledBlock( Vector2 worldPosition, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block == null )
			{
				instructions = null;
				return false;
			}

			instructions = new DemolishInstructions( _builder, _currentCell );
			return true;
		}

		public bool TryQueryEmptyBlock( Vector2 worldPosition, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out var cellData ) || cellData.Block != null )
			{
				instructions = null;
				return false;
			}

			instructions = new RefurbishInstructions( _builder, _currentCell );
			return true;
		}

		public bool TryQueryAnyBlock( Vector2 worldPosition, out SiteInstructions instructions )
		{
			if ( !TryQueryNewBlock( worldPosition, out _ ) )
			{
				instructions = null;
				return false;
			}

			instructions = new SiteInstructions( _builder, _currentCell );
			return true;
		}

		private bool TryQueryNewBlock( Vector2 worldPosition, out LevelCell data )
		{
			if ( _builder.TryGetCellData( worldPosition, out data ) && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}
	}
}
