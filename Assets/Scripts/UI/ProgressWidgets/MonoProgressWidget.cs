using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoProgressWidget : MonoBehaviour,
		IProgressWidget
	{
		public abstract float NormalizedProgress { get; }

		[SerializeField] private bool _isInverted;

		[Button]
		public void SetProgress( float normalizedProgress )
		{
			float progress = _isInverted
				? 1 - normalizedProgress
				: normalizedProgress;

			SetProgress_Internal( progress );
		}

		protected abstract void SetProgress_Internal( float normalizedProgress );
	}
}