using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public abstract class MonoToggleWidget : MonoBehaviour,
		IToggleWidget
	{
		public abstract void Activate();
		public abstract void Deactivate();
	}
}
