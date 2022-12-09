using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelForeman
	{
		public abstract class InternalInstructions
		{
			public LevelCell Cell => _currentCell;

			protected readonly LevelBuilder _builder;
			protected readonly LevelCell _currentCell;

			public InternalInstructions( LevelBuilder builder,
				LevelCell currentCell )
			{
				_builder = builder;
				_currentCell = currentCell;
			}
		}

		public class DemolishInstructions : InternalInstructions
		{
			public DemolishInstructions( LevelBuilder builder, LevelCell currentCell ) : base( builder, currentCell )
			{
			}

			// TODO: Rename to Remove() and then create a Kill() method which will utilize the IDamageable API.
				// This will allow for gems to spawn from Killed blocks when spiders pass them over.
			public RefurbishInstructions Destroy()
			{
				var cellCoord = _currentCell.CellCoord;

				_currentCell.Block.Cleanup();
				_builder.RemoveBlock( cellCoord.Row(), cellCoord.Col() );

				return new RefurbishInstructions( _builder, _currentCell );
			}
		}

		public class RefurbishInstructions : InternalInstructions
		{
			public RefurbishInstructions( LevelBuilder builder, LevelCell currentCell ) : base( builder, currentCell )
			{
			}

			public DemolishInstructions Create( Block.Type type )
			{
				_builder.CreateBlock( type, _currentCell );

				return new DemolishInstructions( _builder, _currentCell );
			}
		}

		public class SiteInstructions : InternalInstructions
		{
			public Vector2 Center => _currentCell.Center;

			public SiteInstructions( LevelBuilder builder, LevelCell currentCell ) : base( builder, currentCell )
			{
			}
		}
	}
}
