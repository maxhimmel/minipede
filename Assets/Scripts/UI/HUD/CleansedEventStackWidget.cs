using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class CleansedEventStackWidget : MonoBehaviour
    {
        [SerializeField] private MonoTickMarkerWidget _tickMarker;

		private CleansingBalanceController.Settings _cleanseEventStack;

		[Inject]
		public void Construct( CleansingBalanceController.Settings cleanseEventStack )
		{
			_cleanseEventStack = cleanseEventStack;
		}

		private void Awake()
		{
			foreach ( var cleanseEvent in _cleanseEventStack.CleansingEvents )
			{
				_tickMarker.PlaceTickMarker( cleanseEvent.Cleansing );
			}
		}
	}
}
