using Minipede.Gameplay.LevelPieces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class LevelCycleTimerWidget : MonoBehaviour
	{
		[SerializeField] private Image _progress;
		[SerializeField] private TMP_Text _cycle;

		private SignalBus _signalBus;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;

			_signalBus.Subscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
			_signalBus.Subscribe<LevelCycleProgressSignal>( OnLevelCycleProgressed );
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<LevelCycleChangedSignal>( OnLevelCycleChanged );
			_signalBus.Unsubscribe<LevelCycleProgressSignal>( OnLevelCycleProgressed );
		}

		private void OnLevelCycleChanged( LevelCycleChangedSignal signal )
		{
			_cycle.text = signal.Cycle.ToString();
		}

		private void OnLevelCycleProgressed( LevelCycleProgressSignal signal )
		{
			_progress.fillAmount = signal.NormalizedProgress;
		}
	}
}