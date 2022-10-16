using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelBlockForeman
	{
		public class DemolishInstructions : InternalInstructions
		{
			public DemolishInstructions( LevelGraph level, LevelCell currentCell ) : base( level, currentCell )
			{
			}

			public RefurbishInstructions Destroy()
			{
				GameObject.Destroy( _currentCell.Block.gameObject );
				_currentCell.Block = null;

				return new RefurbishInstructions( _level, _currentCell );
			}
		}

		public class RefurbishInstructions : InternalInstructions
		{
			public RefurbishInstructions( LevelGraph level, LevelCell currentCell ) : base( level, currentCell )
			{
			}

			public DemolishInstructions Create( Block.Type type )
			{
				_level.CreateBlock( type, _currentCell );

				return new DemolishInstructions( _level, _currentCell );
			}
		}

		public class SiteInstructions : InternalInstructions
		{
			public Vector2 Center => _currentCell.Center;

			public SiteInstructions( LevelGraph level, LevelCell currentCell ) : base( level, currentCell )
			{
			}
		}

		public abstract class InternalInstructions
		{
			protected readonly LevelGraph _level;
			protected readonly LevelCell _currentCell;

			public InternalInstructions( LevelGraph level,
				LevelCell currentCell )
			{
				_level = level;
				_currentCell = currentCell;
			}
		}
	}
}
