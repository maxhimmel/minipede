using Minipede.Gameplay.Player;
using Minipede.Gameplay.Treasures;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class GemCraftWidget : MonoBehaviour
    {
		[SerializeField] private Button _button;

		private ResourceType _resource;
		private Inventory _inventory;

		[Inject]
		public void Construct( ResourceType resource,
			Inventory inventory )
		{
			_resource = resource;
			_inventory = inventory;

			_button.onClick.AddListener( () =>
			{
				_inventory.SelectResourceType( _resource );
			} );
		}
	}
}
