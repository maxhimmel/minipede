using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Enemies
{
	[CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Enemies/Balance Table" )]
	public class EnemyBalances : ScriptableObject
	{
		public CurveEvaluator Speed => _speed;
		public CurveEvaluator Health => _health;

		[SerializeField] private CurveEvaluator _speed = new CurveEvaluator( AnimationCurve.Linear( 0, 1, 10, 1 ) );
		[SerializeField] private CurveEvaluator _health = new CurveEvaluator( AnimationCurve.Linear( 0, 0, 10, 0 ) );
	}
}