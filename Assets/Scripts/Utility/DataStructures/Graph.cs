using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class Graph<TData>
    {
		public delegate TData CreateCell( int row, int col );

		public int Count => _grid.Count;

		private readonly List<Cell> _grid;
		private readonly Vector2Int _dimensions;

        public Graph( int rows, int columns, CreateCell creatingCell = null )
		{
			_grid = new List<Cell>( rows * columns );
			_dimensions = new Vector2Int( rows, columns );

			CreateGrid( creatingCell );
		}

		private void CreateGrid( CreateCell creatingCell = null )
		{
			for ( int row = 0; row < _dimensions.Row(); ++row )
			{
				for ( int col = 0; col < _dimensions.Col(); ++col )
				{
					Cell newCell = creatingCell != null
						? new Cell( row, col, creatingCell( row, col ) )
						: new Cell( row, col );

					_grid.Add( newCell );
				}
			}

			AssignNeighbors();
		}

		private void AssignNeighbors()
		{
			for ( int idx = 0; idx < _grid.Count; ++idx )
			{
				Cell cell = GetCell( idx );
				List<Cell> neighbors = new List<Cell>();

				Vector2Int pos = cell.GetGridPosition();

				// Top ...
				if ( pos.Row() + 1 < _dimensions.Row() )
				{
					neighbors.Add( GetCell( pos.Row() + 1, pos.Col() ) );
					//neighbors.Add( GetCell( pos + new Vector2Int( 1, 0 ) ) );
				}
				// Right ...
				if ( pos.Col() + 1 < _dimensions.Col() )
				{
					neighbors.Add( GetCell( pos.Row(), pos.Col() + 1 ) );
					//neighbors.Add( GetCell( pos + new Vector2Int( 0, 1 ) ) );
				}
				// Bottom ...
				if ( pos.Row() - 1 >= 0 )
				{
					neighbors.Add( GetCell( pos.Row() - 1, pos.Col() ) );
					//neighbors.Add( GetCell( pos - new Vector2Int( 1, 0 ) ) );
				}
				// Left ...
				if ( pos.Col() - 1 >= 0 )
				{
					neighbors.Add( GetCell( pos.Row(), pos.Col() - 1 ) );
					//neighbors.Add( GetCell( pos - new Vector2Int( 0, 1 ) ) );
				}

				cell.SetNeighbors( neighbors );
			}
		}

		public Cell GetCell( int row, int col )
		{
			int index = col + row * _dimensions.Col();
			if ( index < 0 || index >= _grid.Count )
			{
				throw new System.IndexOutOfRangeException( $"({row}, {col}) is not a valid grid position (row, col)." );
			}

			return GetCell( index );
		}

		public Cell GetCell( int index )
		{
			return _grid[index];
		}

		public IEnumerable<Cell> GetColumn( int column, Vertical direction = Vertical.Down )
		{
			if ( column < 0 || column >= _dimensions.Col() )
			{
				throw new System.ArgumentOutOfRangeException( nameof( column ) );
			}

			if ( direction == Vertical.Down )
			{
				for ( int row = _dimensions.Row() - 1; row >= 0; --row )
				{
					yield return GetCell( row, column );
				}
			}
			else
			{
				for ( int row = 0; row < _dimensions.Row(); ++row )
				{
					yield return GetCell( row, column );
				}
			}
		}

		public IEnumerable<Cell> GetRow( int row, Horizontal direction = Horizontal.Right )
		{
			if ( row < 0 || row >= _dimensions.Row() )
			{
				throw new System.ArgumentOutOfRangeException( nameof( row ) );
			}

			if ( direction == Horizontal.Right )
			{
				for ( int column = 0; column < _dimensions.Col(); ++column )
				{
					yield return GetCell( row, column );
				}
			}
			else
			{
				for ( int column = _dimensions.Col() - 1; column >= 0; --column )
				{
					yield return GetCell( row, column );
				}
			}
		}

		public enum Vertical
		{
			Down,
			Up
		}
		public enum Horizontal
		{
			Right, Left
		}
	}
}
