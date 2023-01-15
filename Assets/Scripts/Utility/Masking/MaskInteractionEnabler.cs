using UnityEngine;

namespace Minipede.Utility
{
    public abstract class MaskInteractionEnabler<TComponent> : MonoBehaviour
        where TComponent : Component
    {
		[SerializeField] private SpriteMaskInteraction _defaultInteraction = SpriteMaskInteraction.VisibleInsideMask;

		private void Awake()
		{
			SetInteraction( _defaultInteraction );
		}

		public void SetInteraction( SpriteMaskInteraction interaction )
		{
			var component = GetMaskedComponent();
			SetInteraction( component, interaction );
		}

		protected virtual TComponent GetMaskedComponent()
		{
			return GetComponent<TComponent>();
		}

		protected abstract void SetInteraction( TComponent maskedComponent, SpriteMaskInteraction interaction );
	}
}
