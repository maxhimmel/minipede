using Sirenix.OdinInspector;

namespace ControllerGlyph.Editor
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