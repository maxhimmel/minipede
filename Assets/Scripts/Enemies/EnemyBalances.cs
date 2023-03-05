using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Balance Table" )]
	public class EnemyBalances : ScriptableObject
	{
		[SerializeField] private CurveEvaluator _speed = new CurveEvaluator( AnimationCurve.Linear( 0, 1, 10, 1 ) );
		[SerializeField] private CurveEvaluator _health = new CurveEvaluator( AnimationCurve.Linear( 0, 0, 10, 0 ) );

		public float GetSpeed( int cycle, float defaultValue )
		{
			return _speed.Evaluate( cycle, defaultValue );
		}

		public int GetHealth( int cycle, int defaultValue )
		{
			return Mathf.FloorToInt( _health.Evaluate( cycle, defaultValue ) );
		}
	}
}