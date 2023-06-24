using Minipede.Gameplay.Player;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.StartSequence
{
	public class SelectableStartSequenceMushroom : MonoBehaviour,
		ISelectable
	{
		public IOrientation Orientation => transform.ToData();

		private SpriteRenderer _spriteToggle;

		private bool _isInteractable;

		[Inject]
		public void Construct( [Inject( Id = "Selector" )] SpriteRenderer spriteToggle )
		{
			_spriteToggle = spriteToggle;
		}

		public bool CanBeInteracted()
		{
			return _isInteractable;
		}

		private void OnTriggerEnter2D( Collider2D collision )
		{
			if ( collision.TryGetComponentFromBody<InteractionSelector>( out var selector ) )
			{
				_isInteractable = true;
				Select();
			}
		}

		public void Select()
		{
			_spriteToggle.enabled = true;
		}

		public void Deselect()
		{
			_spriteToggle.enabled = false;
		}
	}
}