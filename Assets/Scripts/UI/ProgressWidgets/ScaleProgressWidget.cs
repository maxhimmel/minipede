using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ScaleProgressWidget : MonoProgressWidget
	{
		public override float NormalizedProgress => _progress;

		[SerializeField] private Transform _scaler;
		[SerializeField] private Vector3 _localStartSize;
		[SerializeField] private Vector3 _localEndSize;

		private float _progress;

		protected override void SetProgress_Internal( float normalizedProgress )
		{
			_progress = normalizedProgress;

			_scaler.localScale = Vector3.Lerp( _localStartSize, _localEndSize, normalizedProgress );
		}
	}
}
