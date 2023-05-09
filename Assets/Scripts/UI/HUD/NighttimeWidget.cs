using Minipede.Gameplay.Fx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Minipede.Gameplay.UI
{
	public class NighttimeWidget : MonoBehaviour
    {
        [SerializeField] private Image _progressMeter;

		[Header( "Animations" )]
		[SerializeField] private Vector2 _animAnchorPos;

		private SignalBus _signalBus;

		private bool _prevNighttimeState;

		[Inject]
		public void Construct( SignalBus signalBus )
		{
			_signalBus = signalBus;
		}

		private void Start()
		{
			_signalBus.Subscribe<NighttimeStateChangedSignal>( OnNighttimeStateChanged );
		}

		private void OnDestroy()
		{
			_signalBus.Unsubscribe<NighttimeStateChangedSignal>( OnNighttimeStateChanged );
		}

		private void OnNighttimeStateChanged( NighttimeStateChangedSignal signal )
		{
			_progressMeter.fillAmount = signal.NormalizedProgress;

			if ( _prevNighttimeState != signal.IsNighttime )
			{
				_prevNighttimeState = signal.IsNighttime;

				RectTransform rectTrans = transform as RectTransform;

				if ( signal.IsNighttime )
				{
					rectTrans.anchoredPosition = _animAnchorPos;
					_signalBus.FireId( "Show", new FxSignal( Vector2.zero, Vector2.zero, null ) );
				}
				else
				{
					rectTrans.anchoredPosition = Vector2.zero;
					_signalBus.FireId( "Hide", new FxSignal( _animAnchorPos, Vector2.zero, null ) );
				}
			}
		}
	}
}
