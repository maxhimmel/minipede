using Minipede.Gameplay.LevelPieces;
using UnityEngine;

namespace Minipede.Utility
{
    public partial class LevelForeman
	{
		public abstract class InternalInstructions
		{
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

			public RefurbishInstructions Destroy()
			{
				GameObject.Destroy( _currentCell.Block.gameObject );
				_currentCell.Block = null;

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
