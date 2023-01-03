using Minipede.Gameplay;
using UnityEngine;
using Zenject;

namespace Minipede.Utility
{
	public class SelectableSpriteToggle : ISelectable
	{
		public IOrientation Orientation => _owner.ToData();

		private Transform _owner;
		private SpriteRenderer _renderer;

		public SelectableSpriteToggle( Transform owner,
			[Inject( Id = "Selector" )] SpriteRenderer renderer )
		{
			_owner = owner;
			_renderer = renderer;
		}

		public bool CanBeInteracted()
		{
			return true;
		}

		public void Deselect()
		{
			_renderer.enabled = false;
		}

		public void Select()
		{
			_renderer.enabled = true;
		}
	}
}