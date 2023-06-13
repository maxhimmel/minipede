using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class DiscProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private Disc _disc;

		[BoxGroup( "Start" )]
		[SerializeField, HideLabel] private LerpData _start = new LerpData() { Time = 0 };
		[BoxGroup( "End" )]
		[SerializeField, HideLabel] private LerpData _end = new LerpData() { Time = 1 };

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			float startAngle = Mathf.LerpUnclamped( _start.StartAngle, _end.StartAngle, normalizedProgress );
			float endAngle = Mathf.LerpUnclamped( _start.EndAngle, _end.EndAngle, normalizedProgress );

			_disc.AngRadiansStart = Mathf.Deg2Rad * startAngle;
			_disc.AngRadiansEnd = Mathf.Deg2Rad * endAngle;
		}

		[System.Serializable]
		private class LerpData
		{
			public float StartAngle => _angle.x;
			public float EndAngle => _angle.y;

			public float Time;
			[MinMaxSlider( -360, 360, ShowFields = true )]
			[SerializeField] private Vector2 _angle;
		}
	}
}