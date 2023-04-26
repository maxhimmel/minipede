using Sirenix.OdinInspector;

namespace Minipede.Editor
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class KeyboardElementId
	{
		[HideLabel]
		public ControllerElementId Element;

		[PropertySpace]
		public int KeyCode;
	}
}