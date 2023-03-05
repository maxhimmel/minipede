using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.LevelPieces
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/MushroomBalanceTable" )]
	public class MushroomBalances : ScriptableObject
	{
		[SerializeField] private CurveEvaluator _health;

		public int GetHealth( int cycle, int defaultValue )
		{
			return Mathf.FloorToInt( _health.Evaluate( cycle, defaultValue ) );
		}
	}
}