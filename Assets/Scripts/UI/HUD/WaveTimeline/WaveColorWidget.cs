using Minipede.Gameplay.Waves;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class WaveColorWidget : MonoBehaviour
	{
		[SerializeField] private MonoColorWidget _palette;

		[HorizontalGroup]
		[ToggleLeft]
		[SerializeField] private bool _overrideCurrentMode;
		
		[HorizontalGroup]
		[EnableIf( "_overrideCurrentMode" ), HideLabel]
		[SerializeField] private Modes _overrideMode;

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
			if ( !_overrideCurrentMode )
			{
				_signalBus.TryUnsubscribe<NighttimeStateChangedSignal>( OnWaveModeChanged );
			}
		}

		private void OnEnable()
		{
			if ( !_overrideCurrentMode )
			{
				_signalBus.Subscribe<NighttimeStateChangedSignal>( OnWaveModeChanged );
			}

			OnWaveModeChanged( new NighttimeStateChangedSignal()
			{
				IsNighttime = _overrideCurrentMode 
					? _overrideMode == Modes.Nighttime
					: _nighttime.IsNighttime
			} );
		}

		private void OnWaveModeChanged( NighttimeStateChangedSignal signal )
		{
			var mode = signal.IsNighttime
				? Modes.Nighttime
				: Modes.Danger;

			var visual = _visuals.GetVisual( mode.ToString() );
			_palette.SetColor( visual.Color );
		}

		private enum Modes
		{
			Danger,
			Nighttime
		}
	}
}