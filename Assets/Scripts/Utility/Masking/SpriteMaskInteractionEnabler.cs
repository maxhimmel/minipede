using UnityEngine;

namespace Minipede.Utility
{
    [RequireComponent( typeof( SpriteRenderer ) )]
    public class SpriteMaskInteractionEnabler : MaskInteractionEnabler<SpriteRenderer>
    {
		protected override void SetInteraction( SpriteRenderer maskedComponent, SpriteMaskInteraction interaction )
		{
			//maskedComponent.maskInteraction = interaction;
		}
	}
}
