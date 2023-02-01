using UnityEngine;
using UnityEngine.EventSystems;

namespace Minipede.Gameplay.UI
{
	public class SetSelected : MonoBehaviour
	{
		private void OnEnable()
		{
			EventSystem.current.SetSelectedGameObject( gameObject );
		}
	}
}