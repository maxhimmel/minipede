using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class ArenaPollutionWidget : MonoBehaviour
    {
        [SerializeField] private MonoProgressWidget _progressFill;
        [SerializeField] private MonoProgressWidget _progressPreview;

		[Header( "Animation" )]
		[SerializeField, MinValue( 0 )] private float _fillDelay = 1;
		[SerializeField, MinValue( 0 )] private float _fillDuration = 1;

		private SignalBus _signalBus;
		private IPollutionWinPercentage _winPercentage;
		private ShipController _shipController;

		private float _prevCleansedPercent;
		private float _cleansedPercent;
		private TaskRunner _progressFillUpdater;

		[Inject]
		public void Construct( SignalBus signalBus,
			IPollutionWinPercentage winPercentage,
			ShipController shipController )
		{
			_signalBus = signalBus;
			_winPercentage = winPercentage;
			_shipController = shipController;

			_progressFillUpdater = new TaskRunner();

			_prevCleansedPercent = _cleansedPercent = 0;

			_progressFill.SetProgress( 0 );
			_progressPreview.SetProgress( 0 );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
			_shipController.Possessed += OnShipPossessed;
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
			_shipController.Possessed -= OnShipPossessed;
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			float offsetMax = 1 - _winPercentage.PollutionWinPercentage;
			float offsetPercent = (signal.NormalizedLevel - _winPercentage.PollutionWinPercentage) / offsetMax;

			_progressPreview.gameObject.SetActive( true );
			_progressPreview.SetProgress( offsetPercent );

			_cleansedPercent = offsetPercent;
		}

		private void OnShipPossessed( Ship obj )
		{
			if ( _fillDelay > 0 || _fillDuration > 0 )
			{
				_progressFillUpdater.Run( UpdateProgressFill ).Forget();
			}
			else
			{
				UpdateProgressFill().Forget();
			}
		}

		private async UniTask UpdateProgressFill()
		{
			if ( _fillDelay > 0 )
			{
				await TaskHelpers.DelaySeconds( _fillDelay, _progressFillUpdater.CancelToken );
			}

			float timer = 0;
			while ( timer < _fillDuration )
			{
				timer += Time.deltaTime;

				float fillValue = Mathf.Lerp( _prevCleansedPercent, _cleansedPercent, timer / _fillDuration );
				_progressFill.SetProgress( fillValue );

				await UniTask.Yield( PlayerLoopTiming.Update, _progressFillUpdater.CancelToken );
			}

			_prevCleansedPercent = _cleansedPercent;
			_progressPreview.gameObject.SetActive( false );
			_progressFill.SetProgress( _cleansedPercent );
		}
	}
}
