using System;
using Minipede.Utility;
using UnityEngine;
using Zenject;

namespace Minipede.Gameplay.Audio
{
    public class MusicPlayer :
		ITickable,
		IDisposable
    {
		private readonly Settings _settings;
		private readonly IAudioController _audioController;

		private float _nextPlayTime;
		private int _nextTrackIndex;
		private IEventInstance _currentEvent;

		public MusicPlayer( Settings settings,
			IAudioController audioController )
		{
			_settings = settings;
			_settings.Tracks.FisherYatesShuffle();

			_audioController = audioController;

			_nextPlayTime = Time.timeSinceLevelLoad + settings.NextTrackDelay;
		}

		public void Dispose()
		{
			if ( _currentEvent != null )
			{
				_currentEvent.Stop();
			}
		}

		public void Tick()
		{
			if ( CanPlayNextTrack() )
			{
				string musicKey = GetNextTrack();
				_currentEvent = _audioController.PlayOneShot( musicKey, Vector2.zero );
			}
		}

		private bool CanPlayNextTrack()
		{
			if ( _nextPlayTime > Time.timeSinceLevelLoad )
			{
				return false;
			}

			return _currentEvent == null || !_currentEvent.IsPlaying;
		}

		private string GetNextTrack()
		{
			if ( _nextTrackIndex >= _settings.Tracks.Length )
			{
				_nextTrackIndex = 0;
				_settings.Tracks.FisherYatesShuffle();
			}

			var eventRef = _settings.Tracks[_nextTrackIndex++];
			return eventRef.EventName;
		}

		[System.Serializable]
		public struct Settings
		{
			public float NextTrackDelay;
			public AudioEventReference[] Tracks;
		}
	}
}
