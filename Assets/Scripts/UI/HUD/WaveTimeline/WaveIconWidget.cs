using Minipede.Gameplay.Waves;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class WaveIconWidget : MonoBehaviour
	{
		[SerializeField] private MonoSpriteWidget _icon;

		private NighttimeController _nighttime;
		private WaveTimelineVisuals _visuals;
		private SignalBus _signalBus;

		[Inject]
		public void Construct( NighttimeController nighttime,
			WaveTimelineVisuals visuals, 
			SignalBus signalBus )
		{
			_nighttime = nighttime;
			_visuals = visuals;
			_signalBus = signalBus;
		}

		private void OnDisable()
		{
			_signalBus.TryUnsubscribe<NighttimeStateChangedSignal>( OnWaveModeChanged );
		}

		private void OnEnable()
		{
			_signalBus.Subscribe<NighttimeStateChangedSignal>( OnWaveModeChanged );

			OnWaveModeChanged( new NighttimeStateChangedSignal()
			{
				IsNighttime = _nighttime.IsNighttime
			} );
		}

		private void OnWaveModeChanged( NighttimeStateChangedSignal signal )
		{
			var mode = signal.IsNighttime
				? Modes.Nighttime
				: Modes.Danger;

			var visual = _visuals.GetVisual( mode.ToString() );
			_icon.SetSprite( visual.Icon );
		}

		private enum Modes
		{
			Danger,
			Nighttime
		}
	}
}