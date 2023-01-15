using UnityEngine;

namespace Minipede.Utility
{
    [RequireComponent( typeof( ParticleSystem ) )]
    public class ParticleMaskInteractionEnabler : MaskInteractionEnabler<ParticleSystemRenderer>
    {
		protected override void SetInteraction( ParticleSystemRenderer maskedComponent, SpriteMaskInteraction interaction )
		{
			maskedComponent.maskInteraction = interaction;
		}
	}
}
