using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class LineProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private Line _line;

		[BoxGroup( "Start" )]
		[SerializeField, HideLabel] private LerpData _start = new LerpData() { Time = 0 };
		[BoxGroup( "End" )]
		[SerializeField, HideLabel] private LerpData _end = new LerpData() { Time = 1 };

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			Vector3 startPos = Vector3.LerpUnclamped( _start.Start, _end.Start, normalizedProgress );
			Vector3 endPos = Vector3.LerpUnclamped( _start.End, _end.End, normalizedProgress );

			_line.Start = startPos;
			_line.End = endPos;
		}

		[System.Serializable]
		private class LerpData
		{
			public float Time;
			public Vector3 Start;
			public Vector3 End;
		}
	}
}