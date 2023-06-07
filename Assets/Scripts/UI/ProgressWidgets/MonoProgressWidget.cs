using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoProgressWidget : MonoBehaviour,
		IProgressWidget
	{
		public abstract float NormalizedProgress { get; }

		public abstract void SetProgress( float normalizedProgress );
	}
}