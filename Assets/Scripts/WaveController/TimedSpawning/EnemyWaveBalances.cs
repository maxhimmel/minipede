using Minipede.Utility;
using UnityEngine;

namespace Minipede.Gameplay.Waves
{
    [CreateAssetMenu( menuName = AppHelper.MenuNamePrefix + "Misc/Wave Balance Table" )]
    public class EnemyWaveBalances : ScriptableObject
    {
        [SerializeField] private CurveEvaluator _frequency = new CurveEvaluator( AnimationCurve.Linear( 0, 0, 10, 0 ) );
        [SerializeField] private CurveEvaluator _swarmAmount = new CurveEvaluator( AnimationCurve.Linear( 0, 0, 10, 0 ) );

        public float GetFrequency( int cycle, float defaultValue )
		{
            return _frequency.Evaluate( cycle, defaultValue );
		}

        public int GetSwarmAmount( int cycle, int defaultValue )
        {
            return Mathf.FloorToInt( _swarmAmount.Evaluate( cycle, defaultValue ) );
        }
    }
}
