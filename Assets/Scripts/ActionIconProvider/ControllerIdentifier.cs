using Sirenix.OdinInspector;

namespace Minipede
{
	[System.Serializable]
	public class ControllerIdentifier
	{
		[BoxGroup( "Controller" ), LabelText( "Editor Name" )]
		public string EditorControllerName;
		[BoxGroup( "Controller" ), LabelText( "Name" )]
		public string ControllerName;
		[BoxGroup( "Controller" ), LabelText( "Guid" )]
		public string ControllerGuid;
	}
}
