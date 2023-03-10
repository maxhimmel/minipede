using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Utility
{
	[System.Serializable]
	public class CurveEvaluator
	{
		[BoxGroup]
		[SerializeField] private Operation _operation;

		[BoxGroup, HideLabel]
		[SerializeField] private AnimationCurve _curve;

		public CurveEvaluator() { }
		public CurveEvaluator( AnimationCurve curve )
		{
			_curve = curve;
		}

		public float Evaluate( float time, float defaultValue )
		{
			switch ( _operation )
			{
				case Operation.Multiply:
					return defaultValue * _curve.Evaluate( time );

				case Operation.Addition:
					return defaultValue + _curve.Evaluate( time );

				case Operation.Subtraction:
					return defaultValue - _curve.Evaluate( time );

				case Operation.Division:
					return defaultValue / _curve.Evaluate( time );

				default:
				case Operation.Override:
					return _curve.Evaluate( time );
			}
		}

		private enum Operation
		{
			Multiply,
			Addition,
			Subtraction,
			Division,
			Override
		}
	}
}