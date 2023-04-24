using Sirenix.OdinInspector;

namespace Minipede.Editor
{
	[HideReferenceObjectPicker]
	[System.Serializable]
	public class MouseElementId
	{
		[HideLabel]
		public InputElementId Element;

		[PropertySpace]
		public string PositiveName;
		public string NegativeName;
	}
}