using Minipede.Gameplay.Waves;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class WaveIconWidget : MonoBehaviour
	{
		[SerializeField] private MonoSpriteWidget _icon;

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
			_dayNightModel.Changed -= OnDayNightChanged;
		}

		private void OnEnable()
		{
			_dayNightModel.Changed += OnDayNightChanged;

			OnDayNightChanged( _dayNightModel );
		}

		private void OnDayNightChanged( DayNightModel model )
		{
			SetIcon( model.IsDaytime ? Modes.Danger : Modes.Nighttime );
		}

		private void SetIcon( Modes mode )
		{
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