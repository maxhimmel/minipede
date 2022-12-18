using Minipede.Gameplay;
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

			/// <summary>
			/// Simple removal of a block.
			/// </summary>
			public RefurbishInstructions Destroy()
			{
				var nextInstruction = RemoveBlock( out var block );				
				block.Cleanup();

				return nextInstruction;
			}

			/// <summary>
			/// Removal of a block through the <see cref="IDamageable"/> API.
			/// </summary>
			public RefurbishInstructions Kill( Transform instigator, Transform causer )
			{
				var nextInstruction = RemoveBlock( out var block );
				block.TakeDamage( instigator, causer, KillInvoker.Kill );
				
				return nextInstruction;
			}

			private RefurbishInstructions RemoveBlock( out Block removedBlock )
			{
				var cellCoord = _currentCell.CellCoord;
				removedBlock = _builder.RemoveBlock( cellCoord.Row(), cellCoord.Col() );

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

		public class AllInstructions : InternalInstructions
		{
			public bool IsEmpty => !IsFilled;
			public bool IsFilled => _currentCell.Block != null;

			public AllInstructions( LevelBuilder builder, LevelCell currentCell ) : base( builder, currentCell )
			{
			}

			public DemolishInstructions Demolish()
			{
				Debug.Assert( IsFilled, $"Cannot create demolish instructions for a cell that isn't filled." );
				return new DemolishInstructions( _builder, _currentCell );
			}

			public RefurbishInstructions Refurbish()
			{
				Debug.Assert( IsEmpty, $"Cannot create refurbish instructions for a cell that isn't empty." );
				return new RefurbishInstructions( _builder, _currentCell );
			}
		}
	}
}
