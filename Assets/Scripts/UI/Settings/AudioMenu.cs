using System.Collections.Generic;
using Minipede.Gameplay.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace Minipede.Gameplay.UI
{
	public class AudioMenu : Menu
	{
		[SerializeField] private Button _backButton;

		[Header( "SFX" )]
		[SerializeField] private Button _reduceSfxButton;
		[SerializeField] private Button _increaseSfxButton;
		[SerializeField] private Image _sfxFill;

		[Header( "Music" )]
		[SerializeField] private Button _reduceMusicButton;
		[SerializeField] private Button _increaseMusicButton;
		[SerializeField] private Image _musicFill;

		[Header( "Settings" )]
		[SerializeField] private float _deltaStep = 0.1f;

		private readonly string[] _mixerKeys = new string[] { "Sfx", "Music" };
		private Dictionary<string, float> _mixerVolumes;
		private Dictionary<string, Image> _mixerFills;

		public override void Initialize()
		{
			base.Initialize();

			InitLookupTables();

			_backButton.onClick.AddListener( _menuController.Pop );

			// SFX ...
			_reduceSfxButton.onClick.AddListener( () => NotifyVolumeChange( "Sfx", -_deltaStep ) );
			_increaseSfxButton.onClick.AddListener( () => NotifyVolumeChange( "Sfx", _deltaStep ) );

			// Music ...
			_reduceMusicButton.onClick.AddListener( () => NotifyVolumeChange( "Music", -_deltaStep ) );
			_increaseMusicButton.onClick.AddListener( () => NotifyVolumeChange( "Music", _deltaStep ) );

			_signalBus.Subscribe<MixerVolumeChangedSignal>( OnMixerVolumeChanged );

			foreach ( var mixerId in _mixerKeys )
			{
				NotifyVolumeChange( mixerId, 0 );
			}
		}

		private void InitLookupTables()
		{
			_mixerVolumes = new Dictionary<string, float>()
			{
				{ "Sfx", 1 },
				{ "Music", 1 }
			};
			_mixerFills = new Dictionary<string, Image>()
			{
				{ "Sfx", _sfxFill },
				{ "Music", _musicFill }
			};
		}

		private void NotifyVolumeChange( string mixer, float delta )
		{
			_signalBus.Fire( new MixerVolumeChangedSignal()
			{
				MixerId = mixer,
				Volume = _mixerVolumes[mixer] + delta
			} );
		}

		private void OnMixerVolumeChanged( MixerVolumeChangedSignal signal )
		{
			_mixerVolumes[signal.MixerId] = signal.Volume;
			_mixerFills[signal.MixerId].fillAmount = signal.Volume;
		}
	}
}