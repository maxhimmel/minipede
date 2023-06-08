using Cysharp.Threading.Tasks;
using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Player;
using Minipede.Utility;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class PollutionWidget : MonoBehaviour
	{
		[BoxGroup( "Progress" )]
		[SerializeField, LabelText( "Preview" )] private MonoProgressWidget _progressPreview;
		[BoxGroup( "Progress" )]
        [SerializeField, LabelText( "Fills" )] private MonoProgressWidget[] _progressFill;

		[Header( "Animation" )]
		[SerializeField] private bool _fillWhenPiloting;
		[SerializeField, MinValue( 0 )] private float _fillDelay = 1;
		[SerializeField, MinValue( 0 )] private float _fillDuration = 1;

		private SignalBus _signalBus;
		private PollutedAreaController _pollutionController;
		private IPollutionWinPercentage _winPercentage;
		private ShipController _shipController;

		private float _prevCleansedPercent;
		private float _cleansedPercent;
		private TaskRunner _progressFillUpdater;

		[Inject]
		public void Construct( SignalBus signalBus,
			PollutedAreaController pollutionController,
			IPollutionWinPercentage winPercentage,
			ShipController shipController )
		{
			_signalBus = signalBus;
			_pollutionController = pollutionController;
			_winPercentage = winPercentage;
			_shipController = shipController;

			_progressFillUpdater = new TaskRunner();

			_prevCleansedPercent = _cleansedPercent = GetPollutionPercent();

			foreach ( var fill in _progressFill )
			{
				fill.SetProgress( 0 );
			}
			_progressPreview.SetProgress( 0 );
		}

		private void OnEnable()
		{
			if ( _fillWhenPiloting )
			{
				_shipController.Possessed += OnShipPossessed;
			}

			_signalBus.Subscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );

			foreach ( var fill in _progressFill )
			{
				fill.SetProgress( GetPollutionPercent() );
			}
		}

		private void OnDisable()
		{
			if ( _fillWhenPiloting )
			{
				_shipController.Possessed -= OnShipPossessed;
			}

			_signalBus.TryUnsubscribe<PollutionLevelChangedSignal>( OnPollutionLevelChanged );
		}

		private void OnShipPossessed( Ship obj )
		{
			StartProgressFill();
		}

		private void OnPollutionLevelChanged( PollutionLevelChangedSignal signal )
		{
			float percent = GetPollutionPercent();

			_progressPreview.gameObject.SetActive( true );
			_progressPreview.SetProgress( percent );

			_cleansedPercent = percent;

			if ( !_fillWhenPiloting )
			{
				StartProgressFill();
			}
		}

		private void StartProgressFill()
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
				foreach ( var fill in _progressFill )
				{
					fill.SetProgress( fillValue );
				}

				await UniTask.Yield( PlayerLoopTiming.Update, _progressFillUpdater.CancelToken );
			}

			_prevCleansedPercent = _cleansedPercent;
			_progressPreview.gameObject.SetActive( false );

			foreach ( var fill in _progressFill )
			{
				fill.SetProgress( _cleansedPercent );
			}
		}

		private float GetPollutionPercent()
		{
			float offsetMax = 1 - _winPercentage.PollutionWinPercentage;
			float offsetPercent = (_pollutionController.PollutionPercentage - _winPercentage.PollutionWinPercentage) / offsetMax;

			return offsetPercent;
		}
	}
}
