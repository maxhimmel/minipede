using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Waves
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/Minipede Wave Balance Table" )]
	public class MinipedeWaveBalances : EnemyWaveBalances
	{
		[SerializeField] private CurveEvaluator _segments = new CurveEvaluator( AnimationCurve.Linear( 0, 0, 10, 0 ) );

		public int GetSegmentCount( int cycle, int defaultValue )
		{
			return Mathf.FloorToInt( _segments.Evaluate( cycle, defaultValue ) );
		}
	}
}