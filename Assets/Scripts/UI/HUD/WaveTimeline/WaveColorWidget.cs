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

		private WaveTimelineVisuals _visuals;
		private DayNightModel _dayNightModel;

		[Inject]
		public void Construct( WaveTimelineVisuals visuals,
			DayNightModel dayNightModel )
		{
			_visuals = visuals;
			_dayNightModel = dayNightModel;
		}

		private void OnDisable()
		{
			if ( !_overrideCurrentMode )
			{
				_dayNightModel.Changed -= OnDayNightChanged;
			}
		}

		private void OnEnable()
		{
			if ( !_overrideCurrentMode )
			{
				_dayNightModel.Changed += OnDayNightChanged;
			}
			else
			{
				SetColor( _overrideMode );
			}
		}

		private void OnDayNightChanged( DayNightModel model )
		{
			SetColor( model.IsDaytime ? Modes.Danger : Modes.Nighttime );
		}

		private void SetColor( Modes mode )
		{
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