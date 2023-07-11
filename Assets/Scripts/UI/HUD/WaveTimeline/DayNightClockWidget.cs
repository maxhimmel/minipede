using Minipede.Gameplay.Waves;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class DayNightClockWidget : MonoBehaviour
	{
		[SerializeField] private MonoProgressWidget _clock;
		[SerializeField] private MonoProgressWidget _daytimeSection;
		[SerializeField] private RotationProgressWidget[] _centerAlignments = new RotationProgressWidget[0];

		private DayNightModel _dayNightModel;

		[Inject]
		public void Construct( DayNightModel dayNightModel )
		{
			_dayNightModel = dayNightModel;

			InitWidgets();
		}

		private void InitWidgets()
		{
			float totalDangerDurationPercent = _dayNightModel.DaytimeDuration / _dayNightModel.TotalDuration;
			_daytimeSection.SetProgress( totalDangerDurationPercent );

			foreach ( var centering in _centerAlignments )
			{
				centering.SetProgress( totalDangerDurationPercent );
			}
		}

		private void OnDisable()
		{
			_dayNightModel.Changed -= OnDayNightChanged;
		}

		private void OnEnable()
		{
			_dayNightModel.Changed += OnDayNightChanged;
		}

		private void OnDayNightChanged( DayNightModel model )
		{
			_clock.SetProgress( model.NormalizedProgress );
		}
	}
}