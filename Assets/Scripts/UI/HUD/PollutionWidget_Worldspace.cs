using Minipede.Gameplay.LevelPieces;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class PollutionWidget_Worldspace : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _cleansedProgressLabel;
        [SerializeField] private MonoProgressWidget _cleansedProgressFill;
        [SerializeField] private MonoProgressWidget _cleansedProgressPreview;

		private SignalBus _signalBus;
		private IPollutionWinPercentage _winPercentage;
		private PollutedAreaController _pollutionController;

		[Inject]
		public void Construct( SignalBus signalBus,
			IPollutionWinPercentage winPercentage,
			PollutedAreaController pollutionController )
		{
			_signalBus = signalBus;
			_winPercentage = winPercentage;
			_pollutionController = pollutionController;

			_cleansedProgressFill.SetProgress( 0 );
			_cleansedProgressPreview.SetProgress( 0 );
			_cleansedProgressLabel.SetProgress( 0 );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );

			OnPollutionLevelChanged( new PollutionLevelChangedSignal()
			{
				CanWin = false,
				NormalizedLevel = _pollutionController.PollutionPercentage
			} );
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			float offsetMax = 1 - _winPercentage.PollutionWinPercentage;
			float offsetPercent = (signal.NormalizedLevel - _winPercentage.PollutionWinPercentage) / offsetMax;
			float percent = Mathf.Clamp01( offsetPercent );

			_cleansedProgressFill.SetProgress( percent );
			_cleansedProgressPreview.SetProgress( 0 );
			_cleansedProgressLabel.SetProgress( percent );
		}
	}
}
