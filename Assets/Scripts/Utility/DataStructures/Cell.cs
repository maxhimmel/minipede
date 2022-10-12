using System.Collections.Generic;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class Graph<TData>
	{
		public class Cell
		{
			public int Row { get; }
			public int Col { get; }

			public TData Item;

			public IReadOnlyList<Cell> Neighbors => _neighbors;

			private List<Cell> _neighbors;

			public Cell( int row, int col, TData item ) : this( row, col )
			{
				Item = item;
			}

			public Cell( int row, int col )
			{
				Row = row;
				Col = col;

				_neighbors = new List<Cell>();
			}

			public void SetNeighbors( List<Cell> neighbors )
			{
				_neighbors = neighbors;
			}

			public Vector2Int GetGridPosition()
			{
				return new Vector2Int( Row, Col );
			}
		}
	}
}
