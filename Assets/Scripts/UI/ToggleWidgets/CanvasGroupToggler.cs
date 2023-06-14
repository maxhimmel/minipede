using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class CanvasGroupToggler : MonoToggleWidget
	{
		[SerializeField] private CanvasGroup _group;

		public override void Activate()
		{
			_group.interactable = true;
		}

		public override void Deactivate()
		{
			_group.interactable = false;
		}
	}
}
