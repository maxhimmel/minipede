using Sirenix.OdinInspector;
using UnityEngine;

namespace Minipede.Gameplay.UI
{
	public class ColorBlinkerWidget : MonoBehaviour
	{
		[SerializeField] private MonoColorWidget _widget;

		[BoxGroup( "Settings" )]
		[SerializeField, HideLabel] private Settings _settings;

		private Color _initialColor;
		private float _timer;

		private void Start()
		{
			_initialColor = _widget.Color;
		}

		private void OnEnable()
		{
			_timer = 0;
		}

		private void Update()
		{
			_timer += _settings.UseUnscaledTime
				? Time.unscaledDeltaTime
				: Time.deltaTime;

			float totalBlinkCycleDuration = 1f / (_settings.BlinksPerSecond * 2f);
			float time = Mathf.PingPong( _timer, totalBlinkCycleDuration );
			float lerp = time / totalBlinkCycleDuration;
			Color newColor = Color.Lerp( _initialColor, _settings.Color, lerp );

			_widget.SetColor( newColor );
		}

		[System.Serializable]
		public class Settings
		{
			public bool UseUnscaledTime;

			[MinValue( 0.01f )]
			public float BlinksPerSecond = 1;

			public Color Color;
		}
	}
}
