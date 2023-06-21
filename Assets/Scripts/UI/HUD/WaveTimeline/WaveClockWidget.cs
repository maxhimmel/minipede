using Minipede.Gameplay.LevelPieces;
using Minipede.Gameplay.Waves;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class WaveClockWidget : MonoBehaviour
	{
		[SerializeField] private MonoProgressWidget _clock;
		[SerializeField] private MonoProgressWidget _dangerSection;
		[SerializeField] private RotationProgressWidget[] _centerAlignments = new RotationProgressWidget[0];

		private LevelCycleTimer.ISettings _cycleSettings;
		private NighttimeController.Settings _nighttimeSettings;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( LevelCycleTimer.ISettings cycleSettings,
			NighttimeController.Settings nighttimeSettings,
			SignalBus signalBus )
		{
			_cycleSettings = cycleSettings;
			_nighttimeSettings = nighttimeSettings;
			_signalBus = signalBus;

			InitWidgets();
		}

		private void InitWidgets()
		{
			float totalDangerDurationPercent = GetTotalDangerDurationPercent();

			_dangerSection.SetProgress( totalDangerDurationPercent );

			foreach ( var centering in _centerAlignments )
			{
				centering.SetProgress( totalDangerDurationPercent );
			}
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<LevelCycleProgressSignal>( OnDangerCycleUpdated );
			_signalBus.TryUnsubscribe<NighttimeStateChangedSignal>( OnNighttimeUpdated );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<LevelCycleProgressSignal>( OnDangerCycleUpdated );
			_signalBus.Subscribe<NighttimeStateChangedSignal>( OnNighttimeUpdated );
		}

		private void OnDangerCycleUpdated( LevelCycleProgressSignal signal )
		{
			int cycle = signal.Cycle % _nighttimeSettings.CycleRate;
			float percentPerCycle = 1f / _nighttimeSettings.CycleRate;
			float cycleOffset = cycle * percentPerCycle;

			float dangerProgress = cycleOffset + Mathf.Lerp( 0, percentPerCycle, signal.NormalizedProgress );

			_clock.SetProgress( GetTotalDangerDurationPercent() * dangerProgress );
		}

		private void OnNighttimeUpdated( NighttimeStateChangedSignal signal )
		{
			float clockProgress = Mathf.Lerp( GetTotalDangerDurationPercent(), 1, signal.NormalizedProgress );
			_clock.SetProgress( clockProgress );
		}

		private float GetTotalDangerDurationPercent()
		{
			return GetTotalDangerDuration() / GetTotalDuration();
		}

		private float GetTotalDuration()
		{
			return _nighttimeSettings.Duration + GetTotalDangerDuration();
		}

		private float GetTotalDangerDuration()
		{
			return _cycleSettings.CycleDuration * _nighttimeSettings.CycleRate;
		}
	}
}