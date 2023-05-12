using ControllerGlyph;
using Rewired;
using Sirenix.OdinInspector;

namespace Minipede.Utility
{
	[System.Serializable]
	public class ControllerGlyphRequest : ControllerGlyphBus.Request
	{
		[BoxGroup]
		[PropertyOrder( -1 )]
		[ActionIdProperty( typeof( ReConsts.Action ) )]
		public int ActionId;

		public override int GetActionId()
		{
			return ActionId;
		}
	}
}