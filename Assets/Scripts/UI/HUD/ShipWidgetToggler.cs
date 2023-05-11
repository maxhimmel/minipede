using Minipede.Gameplay.Player;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ShipWidgetToggler : MonoBehaviour
    {
		[SerializeField] private bool _startEnabled;

		private ShipController _shipController;

        [Inject]
		public void Construct( ShipController shipController )
		{
			_shipController = shipController;
		}

		private void Start()
		{
			_shipController.Possessed += OnShipPossessed;
			_shipController.UnPossessed += OnShipUnPossessed;

			if ( !_startEnabled )
			{
				Hide();
			}
		}

		private void OnDestroy()
		{
			_shipController.Possessed -= OnShipPossessed;
			_shipController.UnPossessed -= OnShipUnPossessed;
		}

		private void OnShipPossessed( Ship obj )
		{
			Show();
		}

		private void Show()
		{
			gameObject.SetActive( true );
		}

		private void OnShipUnPossessed()
		{
			Hide();
		}

		private void Hide()
		{
			gameObject.SetActive( false );
		}
	}
}
