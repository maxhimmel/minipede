using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class RotationProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private Transform _rotator;

		[BoxGroup]
		[SerializeField] private Vector3 _startAngle = new Vector3( 0, 0, 0 );
		[SerializeField] private Vector3 _endAngle = new Vector3( 0, 0, 360 );

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			_rotator.localEulerAngles = Vector3.Lerp( _startAngle, _endAngle, normalizedProgress );
		}
	}
}
