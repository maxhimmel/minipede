using Minipede.Gameplay.Enemies.Spawning;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
    public class WaveTimelineWidget : MonoBehaviour
    {
        [SerializeField] private RectTransform _waveContainer;
        [SerializeField] private WaveItem _wavePrefab;

		private SignalBus _signalBus;
		private WaveTimelineVisuals _timelineVisuals;

		[Inject]
		public void Construct( SignalBus signalBus,
			WaveTimelineVisuals timelineVisuals )
		{
            _signalBus = signalBus;
			_timelineVisuals = timelineVisuals;
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<WaveQueueChangedSignal>( OnWaveQueueChanged );
			_signalBus.Subscribe<WaveProgressSignal>( OnWaveProgressed );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<WaveQueueChangedSignal>( OnWaveQueueChanged );
			_signalBus.TryUnsubscribe<WaveProgressSignal>( OnWaveProgressed );
		}

		private void OnWaveQueueChanged( WaveQueueChangedSignal signal )
		{
			foreach ( Transform child in _waveContainer )
			{
				Destroy( child.gameObject );
			}

			for ( int idx = 0; idx < signal.IdQueue.Count; ++idx )
			{
				var waveItem = Instantiate( _wavePrefab, _waveContainer );
				waveItem.SetProgressActive( idx == 0 );
				waveItem.SetProgression( 0 );

				var id = signal.IdQueue[idx];
				var icon = string.IsNullOrEmpty( id )
					? Color.grey
					: _timelineVisuals.GetVisual( id ).Color;

				waveItem.SetIcon( icon );
			}
		}

		private void OnWaveProgressed( WaveProgressSignal signal )
		{
			//Debug.Log( $"{signal.Id} : {signal.NormalizedProgress:P1}" );

			var firstChild = _waveContainer.GetChild( 0 );
			var currentWave = firstChild.GetComponent<WaveItem>();

			currentWave.SetProgression( signal.NormalizedProgress );
		}
	}
}
