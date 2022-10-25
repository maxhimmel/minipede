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

			instructions = new DemolishInstructions( _builder, _currentCell );
			return true;
		}

		public bool TryQueryFilledBlock( Vector2Int cellCoord, out DemolishInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out var cellData ) || cellData.Block == null )
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

		public bool TryQueryEmptyBlock( Vector2Int cellCoord, out RefurbishInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out var cellData ) || cellData.Block != null )
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

		public bool TryQueryAnyBlock( Vector2Int cellCoord, out SiteInstructions instructions )
		{
			if ( !TryQueryNewBlock( cellCoord, out _ ) )
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

		private bool TryQueryNewBlock( Vector2Int cellCoord, out LevelCell data )
		{
			data = _builder.GetCellData( cellCoord.Row(), cellCoord.Col() );
			if ( data != null && _currentCell != data )
			{
				_currentCell = data;
				return true;
			}

			return false;
		}
	}
}
