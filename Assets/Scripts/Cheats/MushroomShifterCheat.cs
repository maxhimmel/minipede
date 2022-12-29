using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Cheats
{
	public class MushroomShifterCheat : ITickable
	{
		private readonly LevelMushroomShifter _shifter;

		public MushroomShifterCheat( LevelMushroomShifter shifter )
		{
			_shifter = shifter;
		}

		public void Tick()
		{
			if ( Input.GetKeyDown( KeyCode.UpArrow ) )
			{
				_shifter.ShiftAll( Vector2Int.up );
			}
			else if ( Input.GetKeyDown( KeyCode.DownArrow ) )
			{
				_shifter.ShiftAll( Vector2Int.down );
			}
			else if ( Input.GetKeyDown( KeyCode.RightArrow ) )
			{
				_shifter.ShiftAll( Vector2Int.right );
			}
			else if ( Input.GetKeyDown( KeyCode.LeftArrow ) )
			{
				_shifter.ShiftAll( Vector2Int.left );
			}
		}
	}
}