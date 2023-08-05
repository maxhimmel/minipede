using Minipede.Gameplay.Fx;
using Minipede.Gameplay.Player;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class InventoryToggleWidget : MonoBehaviour
	{
		[SerializeField] private bool _isVisibleOnStart;

		[BoxGroup( "FX" )]
		[SerializeField] private Vector2 _showPosition;
		[BoxGroup( "FX" )]
		[SerializeField] private Vector2 _hidePosition;

		private SignalBus _signalBus;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<ToggleInventorySignal>( OnShowInventory );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<ToggleInventorySignal>( OnShowInventory );
		}

		private void Start()
		{
			OnShowInventory( new ToggleInventorySignal() { IsVisible = _isVisibleOnStart } );
		}

		private void OnShowInventory( ToggleInventorySignal signal )
		{
			if ( signal.IsVisible )
			{
				_signalBus.FireId( "Show", new FxSignal( _showPosition, Vector2.zero, null ) );
			}
			else
			{
				_signalBus.FireId( "Hide", new FxSignal( _hidePosition, Vector2.zero, null ) );
			}
		}
	}
}